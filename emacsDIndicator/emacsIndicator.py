#!/usr/bin/env python3

import gi
gi.require_version('Gtk', '3.0')
gi.require_version('AppIndicator3', '0.1')
gi.require_version('Notify', '0.7')

from gi.repository import GLib, Gtk, GObject
from gi.repository import AppIndicator3
from gi.repository import Notify

from sys import argv

checkEmacs = ["emacsclient","-e","0"]
emacsDaemonCommand = ["emacs", "--daemon"]
killCommand = ["emacsclient", "-e", "(kill-emacs)"]
emacsClientCommand = ["emacsclient", "-c"]

startDaemon = True

def printHelpMessage():
    print("""
    Usage: ./emacsIndicator.py [options]
    Options:
      -s         Don't start server
      -h         Print this help message"""[1:])
    #TODO: allow custom commands
    exit(0)

def takeArgs():
    global startDaemon

    args = argv[1:]
    i = 0
    while i < len(args):
        arg = args[i]
        if arg == "-h" or arg == "--help":
            printHelpMessage()
        elif arg == "-s":
            startDaemon = False
        else:
            print("ERROR: unrecognized option: ", arg)
            exit(1)
        i+=1

from os.path import abspath

APPINDICATOR_ID = "emacsDaemonIndicator"
ONLINE_ICON = abspath("emacsOnline.svg")
OFFLINE_ICON = abspath("emacsOffline.svg")
STARTING_ICON = abspath("emacsStarting.svg")

indicator = None
online = False
leaveOpen = False

from threading import enumerate as enumerate_threds, current_thread

def main():
    global indicator, online, workerThread, leaveOpen, startDaemon
    takeArgs()

    GObject.threads_init()

    indicator = AppIndicator3.Indicator.new(APPINDICATOR_ID, OFFLINE_ICON, AppIndicator3.IndicatorCategory.SYSTEM_SERVICES)
    indicator.set_status(AppIndicator3.IndicatorStatus.ACTIVE)
    indicator.set_menu(menu_build())
    Notify.init(APPINDICATOR_ID)

    checkIfOnline()
    if online:
        showMenuAsOnline()
    else:
        showMenuAsOffline()

    if not online and startDaemon:
        start_restart_option(None)

    onlineChecker = Thread(target=keepCheckingOnline, args=[5]) #Checks every five seconds
    onlineChecker.daemon = True
    onlineChecker.start()

    Gtk.main()

    print("Emacs Daemon indicator ============= Quitting...")
    if workerThread != None:
        workerThread.join()

    if leaveOpen:
        print("Emacs Daemon indicator ============= Now we just wait for everything else to exit")
    elif online:
        for thread in clientThreads:
            thread.join()
        stopServer()

START = "Start Emacs Server"
RESTART = "Restart Emacs Server"
STOP = "Stop Emacs Server"
STOPPING = "Stopping..."
STARTING = "Starting..."
RESTARTING = "Restarting..."

def useRestartName(restart):
    global item_startServer
    item_startServer.set_label(RESTART if restart else START)


def menu_build():
    global item_startServer, item_stopServer, item_startClient, item_quitButLeave

    menu = Gtk.Menu()

    item_startServer = Gtk.MenuItem("")
    item_startServer.connect('activate', start_restart_option)
    menu.append(item_startServer)

    item_stopServer = Gtk.MenuItem(STOP)
    item_stopServer.connect('activate', stop_server_option)
    menu.append(item_stopServer)

    item_startClient = Gtk.MenuItem("Start Client")
    item_startClient.connect('activate', start_client_option)
    menu.append(item_startClient)

    item_quitButton = Gtk.MenuItem("Quit")
    item_quitButton.connect('activate', quit_option)
    menu.append(item_quitButton)

    item_quitButLeave = Gtk.MenuItem("Quit but leave Emacs")
    item_quitButLeave.connect('activate', quit_but_leave)
    menu.append(item_quitButLeave)

    menu.show_all()

    return menu

from threading import Thread

workerThread = None

def start_restart_option(source):
    global online, workerThread
    if workerThread != None:
        workerThread.join()
    workerThread = Thread(target=restartServer if online else launchServer)
    workerThread.start()

def stop_server_option(source):
    global workerThread
    if workerThread != None:
        workerThread.join()
    workerThread = Thread(target=stopServer)
    workerThread.start()

clientThreads = []

def start_client_option(source):
    global workerThread, clientThreads
    if workerThread != None:
        workerThread.join()
    assert(online)
    clientThread = Thread(target=startClient)
    clientThread.daemon = True
    clientThread.start()
    clientThreads.append(clientThread)

def quit_option(source):
    global indicator
    indicator.set_icon(STARTING_ICON)
    Notify.uninit()
    GLib.idle_add(Gtk.main_quit)

def quit_but_leave(source):
    global leaveOpen
    leaveOpen = True
    quit_option(source)

from subprocess import run

def startClient():
    global clientThreads
    if run(emacsClientCommand).returncode != 0:
        Notify.Notification.new("Failed to start client", "Something went wrong", "emacs").show()
    clientThreads.remove(current_thread())

from subprocess import PIPE

def checkIfOnline():
    global online
    online = run(checkEmacs, stdout=PIPE, stderr=PIPE).returncode == 0

from time import sleep

def keepCheckingOnline(ONLINE_CHECK_DELAY):
    while True:
        sleep(ONLINE_CHECK_DELAY)
        oldOnline = online
        checkIfOnline()
        if oldOnline != online:
            if online:
                GLib.idle_add(showMenuAsOnline)
            else:
                GLib.idle_add(showMenuAsOffline)

def showMenuAsOffline():
    global indicator
    indicator.set_icon(OFFLINE_ICON)
    useRestartName(False) #Use start
    item_startServer.set_sensitive(True)
    item_stopServer.set_sensitive(False)
    item_startClient.set_sensitive(False)
    item_quitButLeave.set_sensitive(False)

def showMenuAsOnline():
    global indicator
    indicator.set_icon(ONLINE_ICON)
    useRestartName(True) #Use restart
    item_startServer.set_sensitive(True)
    item_stopServer.set_sensitive(True)
    item_startClient.set_sensitive(True)
    item_quitButLeave.set_sensitive(True)

def launchServer():
    global online, instance
    assert(not online)
    GLib.idle_add(lambda: indicator.set_icon(STARTING_ICON))
    GLib.idle_add(lambda: item_startServer.set_label("Starting..."))
    GLib.idle_add(lambda: item_startServer.set_sensitive(False))
    print("Emacs Daemon indicator ============= Starting server")

    code = run(emacsDaemonCommand).returncode

    if code != 0:
        Notify.Notification.new("Starting the Emacs server failed", "Probably a problem on your end", None).show()
        GLib.idle_add(showMenuAsOffline)
    else:
        online = True
        GLib.idle_add(showMenuAsOnline)

def beforeStopGUIupdate():
    indicator.set_icon(STARTING_ICON)
    item_startServer.set_sensitive(False)
    item_stopServer.set_sensitive(False)
    item_stopServer.set_label(STOPPING)
    item_startClient.set_sensitive(False)
    item_quitButLeave.set_sensitive(False)

def stopServer():
    global online
    assert(online)
    GLib.idle_add(beforeStopGUIupdate)
    print("Emacs Daemon indicator ============= Stopping server")

    if online: #Asserted
        if run(killCommand).returncode != 0:
            Notify.Notification.new("Failed to stop", "That ain't pretty", None).show()

    online = False

    GLib.idle_add(lambda: item_stopServer.set_label(STOP)) #Back to the stop command
    GLib.idle_add(showMenuAsOffline)

def restartServer():
    GLib.idle_add(lambda: item_startServer.set_label(RESTARTING))
    stopServer()
    launchServer()

if __name__ == "__main__":
    main()

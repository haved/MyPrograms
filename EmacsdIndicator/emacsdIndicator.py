#!/usr/bin/env python3

import gi
gi.require_version('Gtk', '3.0')
gi.require_version('AppIndicator3', '0.1')
gi.require_version('Notify', '0.7')

from gi.repository import GLib, Gtk, GObject
from gi.repository import AppIndicator3
from gi.repository import Notify

from sys import argv

from os.path import dirname, join as joinpath, abspath, expanduser

APPINDICATOR_ID = "emacsDaemonIndicator"

thisDir = dirname(__file__)
ONLINE_ICON = abspath(joinpath(thisDir, "emacsOnline.svg"))
OFFLINE_ICON = abspath(joinpath(thisDir, "emacsOffline.svg"))
WORKING_ICON = abspath(joinpath(thisDir, "emacsWorking.svg"))

runDaemonOnStart = True

daemonRunning = False
working = False

def main():
    print("Emacs Daemon indicator == Starting...")

    GObject.threads_init()
    Notify.init(APPINDICATOR_ID)

    skipToEnd = False
    for arg in argv[1:]:
        if arg == '--help':
            printHelpMessage()
            skipToEnd = True
            break
        elif arg == '-w':
            global runDaemonOnStart
            runDaemonOnStart = False
        elif arg == '-c':
            tryNewClient()
            skipToEnd = True
            break

    if not skipToEnd:
        global indicator
        indicator = AppIndicator3.Indicator.new(APPINDICATOR_ID, OFFLINE_ICON, AppIndicator3.IndicatorCategory.SYSTEM_SERVICES)

        checkIfRunning()
        displayState()
        if not daemonRunning and runDaemonOnStart:
            start_option(None)

        indicator.set_status(AppIndicator3.IndicatorStatus.ACTIVE)
        print("Emacs Daemon indicator == Ready")
        Gtk.main()

    print("Emacs Daemon indicator == Quitting...")
    Notify.uninit()

from subprocess import PIPE, run

def checkIfRunning():
    global daemonRunning
    daemonRunning = run(["emacsclient", "-e", "0"], stdout=PIPE, stderr=PIPE).returncode == 0

emacsDaemonCommand = ["emacs", "--daemon"]
def startServer():
    global daemonRunning
    checkIfRunning()
    if daemonRunning:
        Nofify.Notification.new("Server already running", "All should be well, though", "emacs").show()
    else:
        print("Emacs Daemon indicator == Starting server...")
        proc = run(emacsDaemonCommand, cwd=expanduser("~"), stdout=PIPE, stderr=PIPE)

        if proc.returncode != 0:
            Notify.Notification.new("Starting the Emacs server failed", proc.stderr.decode('utf-8'), "emacs").show()
        else:
            checkIfRunning()
            if not daemonRunning:
                Notify.Notification.new("The server doesn't accept connections", "Although it started with a return code of 0", "emacs").show()

killCommand = ["emacsclient", "-e", "(kill-emacs)"]
def stopServer():
    global daemonRunning
    checkIfRunning()
    if not daemonRunning:
        Notify.Notification.new("The emacs server wasn't running after all", "Consider it killed", "emacs").show()
    else:
        print("Emacs Daemon indicator == Killing server...")
        proc = run(killCommand, stdout=PIPE, stderr=PIPE)

        if proc.returncode != 0:
            Notify.Notification.new("Failed killing the emacs server", proc.stderr.decode('utf-8'), "emacs").show()
        else:
            checkIfRunning()
            if daemonRunning:
                Notify.Notification.new("Um... the killing was successfull", "But the server is still running", "emacs").show()

startClientCommand = ["emacsclient", "-c"]
def newClient():
    global daemonRunning
    checkIfRunning()
    tellGtkToDisplayState()
    if not daemonRunning:
        Notify.Notification.new("The emacs server isn't running", "Failed to start client", "emacs").show()
    else:
        print("Emacs Daemon indicator == Starting client...")
        proc = run(startClientCommand, stdout=PIPE, stderr=PIPE)
        if proc.returncode != 0:
            Notify.Notification.new("Client failed with error code " + str(proc.returncode), proc.stderr.decode('utf-8'), "emacs").show()

from threading import Lock, Thread
workLock = Lock()
#Must be gtk thread
def doWork(operation):
    global working, workLock

    workLock.acquire()

    working = True
    displayState()

    def threadWork():
        global working, workLock
        operation()
        working = False
        tellGtkToDisplayState()
        workLock.release()

    workingThread = Thread(target = threadWork)
    workingThread.start()

#Must be gtk thread
def start_option(source):
    doWork(startServer)

#Must be gtk thread
def stop_option(source):
    doWork(stopServer)

#Must be gtk thread
def restart_option(source):
    def stopThenStart():
        stopServer()
        startServer()
    doWork(stopThenStart)


#Must be gtk thread
def check_option(source):
    doWork(checkIfRunning)

#Must be gtk thread
def start_client_option(source):
    Thread(target=newClient).start()

#Must be gtk thread
def quit_option(source):
    global working
    working = True
    displayState()
    GLib.idle_add(Gtk.main_quit)

#Must be gtk thread
def displayState():
    global daemonRunning, working, indicator
    if working:
        indicator.set_menu(get_menus()['working'])
        indicator.set_icon(WORKING_ICON)
    elif daemonRunning:
        indicator.set_menu(get_menus()['online'])
        indicator.set_icon(ONLINE_ICON)
    else:
        indicator.set_menu(get_menus()['offline'])
        indicator.set_icon(OFFLINE_ICON)

def tellGtkToDisplayState():
    GLib.idle_add(displayState)

#Must be gtk thread
def get_menus():
    global GLOBAL_MENUS

    if 'GLOBAL_MENUS' in globals():
        return GLOBAL_MENUS

    print("Making all menus")

    offline_menu = Gtk.Menu()
    working_menu = Gtk.Menu();
    online_menu = Gtk.Menu();

    # =========== Offline =========
    item_startServer = Gtk.MenuItem("Start emacs server")
    item_startServer.connect('activate', start_option)
    offline_menu.append(item_startServer)

    item_checkServer = Gtk.MenuItem("Check server")
    item_checkServer.connect('activate', check_option)
    offline_menu.append(item_checkServer)

    item_quit = Gtk.MenuItem("Quit")
    item_quit.connect('activate', quit_option)
    offline_menu.append(item_quit)

    offline_menu.show_all()

    # =========== Working =========
    item_dummy_working = Gtk.MenuItem("Working...")
    item_dummy_working.set_sensitive(False)
    working_menu.append(item_dummy_working)

    item_quit = Gtk.MenuItem("Quit")
    item_quit.connect('activate', quit_option)
    working_menu.append(item_quit)

    working_menu.show_all()

    # =========== Online ==========
    item_stopServer = Gtk.MenuItem("Stop server")
    item_stopServer.connect('activate', stop_option)
    online_menu.append(item_stopServer)

    item_restartServer = Gtk.MenuItem("Restart server")
    item_restartServer.connect('activate', restart_option)
    online_menu.append(item_restartServer)

    item_quit = Gtk.MenuItem("Check")
    item_quit.connect('activate', check_option)
    online_menu.append(item_quit)

    item_startClient = Gtk.MenuItem("Start client")
    item_startClient.connect('activate', start_client_option)
    online_menu.append(item_startClient)

    item_quit = Gtk.MenuItem("Quit")
    item_quit.connect('activate', quit_option)
    online_menu.append(item_quit)

    online_menu.show_all()

    # ========== Put them all in the global =========
    GLOBAL_MENUS = {'offline': offline_menu, 'working': working_menu, 'online': online_menu}
    return GLOBAL_MENUS

def printHelpMessage():
    print("""Usage: ./emacsdIndicator.py <options>
    A notification area applet for monitoring, starting and stopping the emacs server
Options:
    --help      Print this help message (and die)
    -w          Don't automatically start the daemon
    -c          Try running a new client (and die)
    """)
    exit(0)

def tryNewClient():
    newClient()

if __name__ == "__main__":
    main()

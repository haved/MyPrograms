#!/usr/bin/env python3

import gi
gi.require_version('Gtk', '3.0')
gi.require_version('AppIndicator3', '0.1')
gi.require_version('Notify', '0.7')

from gi.repository import GLib, Gtk, GObject
from gi.repository import AppIndicator3
from gi.repository import Notify

from sys import argv

from os.path import abspath

APPINDICATOR_ID = "emacsDaemonIndicator"
ONLINE_ICON = abspath("emacsOnline.svg")
OFFLINE_ICON = abspath("emacsOffline.svg")
WORKING_ICON = abspath("emacsWorking.svg")

runDaemonOnStart = False

daemonRunning = False
working = False

def main():
    print("Emacs Daemon indicator == Starting...")
    GObject.threads_init()
    Notify.init(APPINDICATOR_ID)

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

def checkIfRunning():
    global daemonRunning
    daemonRunning = False #TODO: Check

#Must be gtk thread
def start_option(source):
    global working
    working = True
    displayState()
    #TODO: Start server

#Must be gtk thread
def stop_option(source):
    global working
    working = True
    displayState()
    #TODO: Stop server

#Must be gtk thread
def restart_option(source):
    global working
    working = True
    displayState()
    #TODO: Restart server


#Must be gtk thread
def check_option(source):
    pass

#Must be gtk thread
def start_client_option(source):
    pass

#Must be gtk thread
def quit_option(source):
    global working
    working = True
    displayState()
    Notify.uninit()
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

#Must be gtk thread
def get_menus():
    global GLOBAL_MENUS

    if 'GLOBAL_MENUS' in globals():
        return GLOBAL_MENUS

    print("Making all menus")

    offline_menu = Gtk.Menu()
    working_menu = Gtk.Menu();
    online_menu = Gtk.Menu();

    item_startServer = Gtk.MenuItem("Start emacs server")
    item_startServer.connect('activate', start_option)
    offline_menu.append(item_startServer)

    item_dummy_working = Gtk.MenuItem("Working...")
    item_dummy_working.set_sensitive(False)
    working_menu.append(item_dummy_working)

    item_stopServer = Gtk.MenuItem("Stop server")
    item_stopServer.connect('activate', stop_option)
    online_menu.append(item_stopServer)

    item_restartServer = Gtk.MenuItem("Restart server")
    item_restartServer.connect('activate', restart_option)
    online_menu.append(item_restartServer)

    item_checkServer = Gtk.MenuItem("Check server")
    item_checkServer.connect('activate', check_option)
    offline_menu.append(item_checkServer)
    online_menu.append(item_checkServer)

    item_startClient = Gtk.MenuItem("Start client")
    item_startClient.connect('activate', start_client_option)
    online_menu.append(item_startClient)

    item_quit = Gtk.MenuItem("Quit")
    item_quit.connect('activate', quit_option)
    offline_menu.append(item_quit)
    working_menu.append(item_quit)
    online_menu.append(item_quit)

    offline_menu.show_all()
    working_menu.show_all()
    online_menu.show_all()

    GLOBAL_MENUS = {'offline': offline_menu, 'working': working_menu, 'online': online_menu}
    return GLOBAL_MENUS

if __name__ == "__main__":
    main()

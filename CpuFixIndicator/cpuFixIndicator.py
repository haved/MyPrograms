#!/usr/bin/env python3

import gi
gi.require_version('Gtk', '3.0')
gi.require_version('AppIndicator3', '0.1')
gi.require_version('Notify', '0.7')

from gi.repository import GLib, Gtk, GObject
from gi.repository import AppIndicator3
from gi.repository import Notify

APPINDICATOR_ID = "cpuFixIndicator"
ICON = "cpu"

def main():
    print("Running cpuFixIndicator")
    Notify.init(APPINDICATOR_ID)

    indicator = AppIndicator3.Indicator.new(APPINDICATOR_ID, ICON, AppIndicator3.IndicatorCategory.SYSTEM_SERVICES)

    menu = Gtk.Menu()
    item_cpuFix = Gtk.MenuItem(label="CpuFix")
    item_cpuFix.connect('activate', cpuFixClicked)
    menu.append(item_cpuFix)

    item_quit = Gtk.MenuItem(label="Quit")
    item_quit.connect('activate', Gtk.main_quit)
    menu.append(item_quit)

    menu.show_all()

    indicator.set_status(AppIndicator3.IndicatorStatus.ACTIVE)
    indicator.set_menu(menu)

    # Run main loop
    Gtk.main()

    print("cpuFixIndicator quitting...")
    Notify.uninit()

from subprocess import run, TimeoutExpired, CalledProcessError
from threading import Thread

counter = 0
def cpuFixClicked(source):
    global counter
    counter += 1

    source.get_child().set_text(f"CpuFix #{counter+1}")

    def callCpuFix():
        try:
            result = run(["sudo", "cpuFix.sh"], capture_output=True, timeout=2, check=True, encoding='utf-8')
            Notify.Notification.new("Cpu should now be fixed!", f"For the {counter}. time", ICON).show()
        except TimeoutExpired as err:
            Notify.Notification.new("CpuFix timed out!", err.stdout + "\n\n" + err.stderr, ICON).show()
        except CalledProcessError as err:
            Notify.Notification.new(f"CpuFix failed! (return code {err.returncode})", err.stdout + "\n\n" + err.stderr, ICON).show()

    Thread(target=callCpuFix).start()

main()

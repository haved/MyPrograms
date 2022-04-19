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

AVERAGE_CPU_MHZ_THRESHOLD = 1000

def main():
    print("Running cpuFixIndicator")
    Notify.init(APPINDICATOR_ID)
    indicator = AppIndicator3.Indicator.new(APPINDICATOR_ID, ICON, AppIndicator3.IndicatorCategory.SYSTEM_SERVICES)

    menu = Gtk.Menu()
    item_cpuFix = Gtk.MenuItem(label="CpuFix")
    item_cpuFix.connect('activate', cpuFixClicked)
    menu.append(item_cpuFix)

    item_monitor = Gtk.CheckMenuItem(label="Live monitor")
    item_monitor.connect('toggled', liveMonitor)
    menu.append(item_monitor)
    item_monitor.set_active(True)

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

counter = 0
def callCpuFix(quiet=False):
    global counter
    counter += 1
    try:
        result = run(["sudo", "cpuFix.sh"], capture_output=True, timeout=2, check=True, encoding='utf-8')
        if not quiet:
            Notify.Notification.new("Cpu should now be fixed!", f"For the {counter}. time", ICON).show()
    except TimeoutExpired as err:
        Notify.Notification.new("CpuFix timed out!", err.stdout + "\n\n" + err.stderr, ICON).show()
    except CalledProcessError as err:
        Notify.Notification.new(f"CpuFix failed! (return code {err.returncode})", err.stdout + "\n\n" + err.stderr, ICON).show()

from subprocess import run, TimeoutExpired, CalledProcessError
from threading import Thread
from time import sleep

def cpuFixClicked(source):
    Thread(target=callCpuFix).start()

monitor_running = False
def monitoringFunction():
    global monitor_running
    try:
        print("Running monitoring thread")
        while monitor_running:
            with open("/proc/cpuinfo", "r") as fil:
                speeds = [float(l.split(':')[1]) for l in fil.readlines() if l.startswith("cpu MHz")]
            avg = sum(speeds) / len(speeds)
            if avg < AVERAGE_CPU_MHZ_THRESHOLD:
                print("Monitor thread detected slow cores!")
                callCpuFix(quiet=True)
            sleep(1)
    except Exception as err:
        Notify.Notification.new("Live monitoring of cpu failed!", str(err), ICON).show()
        monitor_running = False

monitor_thread = None
def liveMonitor(source):
    global monitor_running, monitor_thread
    state = source.get_active()
    print(f"Live monitor now: {state}")

    if state and not monitor_running:
        monitor_running = True
        monitor_thread = Thread(target=monitoringFunction, daemon=True)
        monitor_thread.start()

    elif not state and monitor_running:
        monitor_running = False
        monitor_thread.join()

main()

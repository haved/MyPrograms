## Emacsd Indicator
This is an indicator applet showing the current state of the Emacs server.
It also allows you to start and stop the server, as well as opening clients.
The red icon means no server is running, grey means booting, and the green means up and running.
If no emacs server is running already, it will start one up by default, changing icon color once the server is ready.
Then you can start emacs clients, which are much faster to open.

#### Usage
 - `./emacsdIndicator.py` start the applet, which will also start an emacs server if none are running
 - `./emacsdIndicator.py -w` same as above but doesn't automatically start any server
 - `./emacsdIndicator.py -c` starts a client or complains if the server isn't running

#### Installation
There is no install script yet. I suggest putting a shortcut in `~/.config/autostart`, as well as a menu item for opening clients in your menu of choice.

#### Dependencies
 * python3
 * libappindicator-gtk3

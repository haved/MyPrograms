## Emacsd Indicator
This is an indicator applet showing the current state of the Emacs server.
It also allows you to start and stop the server, as well as opening clients.
The red icon means no server is running, grey means booting, and the green means up and running.

#### Usage
 - `./emacsdIndicator.py` start the applet, which will also start an emacs server if none are running
 - `./emacsdIndicator.py -w` same as above but doesn't automatically start the server
 - `./emacsdIndicator.py -c` starts a client or complains if the server isn't running

#### Installation
There is no install script yet :(

#### Dependencies
 * python3
 * libappindicator-gtk3

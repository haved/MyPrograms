## CpuFixIndicator

A short notification area applet to quickly run my `cpuFix.sh` script, in case the BIOS resets the TDP settings.

#### Dependencies
 * python3
 * libappindicator-gtk3
 * a `cpuFix.sh` script on PATH, which must have passwordless sudo
 
#### Passwordless sudo
Use `sudo visudo` to append the line as late as possible
```
MY_USERNAME ALL=(root) NOPASSWD: /path/to/cpuFix.sh
```

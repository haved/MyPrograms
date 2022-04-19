#!/usr/bin/bash -e
echo "Unbinding driver"
echo -n "0000:04:00.3" | tee /sys/bus/pci/drivers/xhci_hcd/unbind
echo -e "\nBinding driver"
echo -n "0000:04:00.3" | tee /sys/bus/pci/drivers/xhci_hcd/bind
echo -e "\nDone!"

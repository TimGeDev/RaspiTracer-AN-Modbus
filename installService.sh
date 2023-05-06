#/bin/bash
sudo cp -v RaspiTracer.service /etc/systemd/system #copy service file
sudo systemctl enable RaspiTracer.service          #enable service
sudo systemctl start RaspiTracer.service           #start service
sudo systemctl status RaspiTracer.service          #check if the service started
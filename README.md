# AdjustDisplay
Console application that allows you to change video modes, resolution, rotation, scale for all connected displays.
Application should work with all types of connected displays, VGA/HDMI/DVI or wireless.
Hovewer, some projection modes can prevent video mode and/or scale changes.
From my tests, I was able to change anything I can change from built-in Windows UI dialogs and little bit more.

## Requrements:
- Windows OSes (tested on Windows 10 with two monitors only)

You can use AdjustDisplay for automation from the standard Windows console ("Command prompt") in the interactive mode or in the batch mode (in the .bat, .cmd files)

## Commands and parameters:

**ident**            : identify displays


**get**              : return connected displays information

**get** disp=n       : return info for all supported modes for the specific display, where **n** starts from 1



**set** [params]     : set parameters for specific display

    disp=n       : display number (starts from 1); if omitted, disp=1 will be used
    
    mode=nn      : set video mode (from range returned by get disp=n)
    
    width=nnnn   : desired screen width, i.e. width=2048
    
    height=nnnn  : desired screen height, i.e. height=1080
    
    orient=nnn   : diserid screen orientation; available values are: 0, 90, 180, 270
    
    freq=nnn     : desired screen frequency (refresh rate)
    
    bpp=nn       : desired bits per pixel; available values are: 8, 16, 24, 32
    
    scale=nnn    : disared screen scale; available values are: [100, 125, 150...450, 500]

Note: any params can be omitted

## Author's note:
I can't test every possible configuration (only tested with two monitors), so reports on running different configurations are most welcome! Please submit your reports, bug reports and feature requests to the "Issues"

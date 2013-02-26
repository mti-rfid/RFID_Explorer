Explorer Projects

Copyright 2011 MTI, Inc.
Updated: July 11, 2011

Description:

This solution is designed to build the source associated with the
Explorer program.  It consists of the following sub-projects:

* Library
----------------------------

  This is the primary lower layer utilized by the Explorer program.
  It is used to communicate with the native RFID Reader Library.
  It has a dependency requirment on version 2 (or greater) of the
  .NET framework.

  Further details concerning this sub-project, are contained
  within the ReadMe.txt file, located in the project directory.


* RFIDInterface
---------------

  This project is an assembly utilized by Explorer that primarily
  provides a variety of ( semi- ) uniform accessor, source and sink
  classes, related to the CSharp versions of the RFID Structures.

  It can also be considered an example of how to access the R1000
  radio using the CSharp Linkage.dll functionality.
  i.e. the files prefixed with Source_ all provide load( ) and
  store( ) functions that show such access.

* Explorer
-------------

  This is the primary Explorer project that instantiates the main GUI
  window and manages all display and form features of the application.

* Explorer Installer
-----------------------

  This project is responsible for bundling up the other projects in
  the solution and creating an installable package.

  Note that in addition to bundling the binary ( dll, etc. ) files
  from other projects, it will also bundle the *.config files from
  the Explorer project.  These *.config files are REQUIRED for proper
  operation of the Explorer program.

Build Notes:
------------

* Several of the projects use a generic signing key 'SigningKey.pfx'
  that may request a password on solution or project entry.  The
  password assigned to ALL of the signing keys is simply 'password'.

* The CSharp NET Linkage - Build Merge Module, references the native
  cpl.dll, rfid.dll and rfidtx.dll files.  If the given locations of
  these files do not match your system, you should delete them from
  the project and then ADD them back in - specifying their correct
  location on your system.

* The reference to RFIDInterface in the Explorer project has been
  observed to occassionally break when the project is moved between
  systems.  If this occurs - signified by an icon with a red slash -
  simply remove the existing reference then:

  - right click the Explorer -> Reference icon in solution explorer
  - select the 'Add Reference' option
  - a dialog box will open, select / click on the 'Projects' tab
  - select RFIDInterface and click the OK button

  The reference should be resolved at this point, and the project
  should compile properly.

Deployment:
-----------

* As mentioned previously, the Explorer Installer project creates the
  installation utility for the Explorer sample application.  In fact
  it produces two files:

  Setup.exe - this file is responsible for checking that all dependencies
  on the target system are met e.g. .NET Framework 2.0 or greater is
  installed.  If dependencies are met then it chains to the msi file
  detailed below.

  Explorer Installer.msi - this is the primary Explorer installer.
  While it can be used directly instead of via Setup.exe, it is strongly
  suggested that Setup.exe is always utilized.

* On deployment the following directories and files should be installed
  on the target system:

  [Program Files]\MTI\MTI Explorer vX.X.X

	app.config
	AppUninstall.exe
	cpl.dll
	ImpinjBanner.jpg
	Linkage.dll
	macErrors.config
	rfid.dll
	RFIDcomm.cfg
	RFIDInterface.dll
	rfidtx.dll
	Explorer.exe
	Explorer.ico
	Uninstall Explorer.lnk
	Uninstall.ico

* Additionally, a shortcut to the Explorer executable will be placed
  on the user's desktop, and in folder:  [Program Files]\MTI Inc;
  so that it may be launced from 'Start' 'All Programs' 'MTI Inc'

Running the Program:
--------------------

  The RFID Explorer application may be started by double-clicking an
  installed shortcut on the desktop, or from under the Start menu.

Notes:
------

  The RFID Explorer sample application defaults to enumerating
  R1000 radios attached via USB.

  To utilize a UART connection, an appropriate configured R1000
  must be attached to a com port, AND the file RFIDcomm.cfg MUST
  be created and placed in the directory:

  [Program Files]\MTI Inc\Explorer

The contents of RFIDcomm.cfg should be the text line:

  Port=N

Where the leading spaces prior to the word Port should be removed AND
N should contain the port number to which the target radio is attached,
i.e. - typically 1, 2, 3, or 4.


C# NET Linkage - Build Project

Copyright 2009 Impinj Inc
Updated: November 18, 2009

Description:
------------

* This project will build the assembly Linkage.dll for platforms
  targeted against the Microsoft .NET Framework v 2.0 and greater.

* The Linkage.dll has been tested against a 'standard' Microsoft
  Windows XP system with service pack 2.

* While untested, it is expected for be suitable for deployment
  against any system utilizing .NET v 2.0 or higher.

Build Notes:
------------

* This project shares the following files with the similarly named
  .NET CF ( Compact Framework ) project:

  Constants.cs
  Structures.cs
  Linkage.cs

  As the above named files are shared, any modifications to them WILL
  result in changes to other projects utilizing them.

* The generic signing key 'SigningKey.pfx' with the password 'password'
  is provided for test use.  While not necessary, it is still suggested
  that users replace this signing key with one of their own for purposes
  of distribution.

* The Linkage.dll has a static constructor provided in the file
  LinkageStaticConstructor.cs.  The constructor is utilized so that the
  required native R1000 libraries ( see below ) may be placed in either
  a well known directory, the same directory as the Linkage.dll file,
  OR in the directory [Linkage.dll directory]\..\clsPacket.

Deployment:
-----------

* This project produces the file Linkage.dll which requires at a minimum,
  the Indy RFID Library binaries cpl.dll, rfidtx.dll and rfid.dll all C++
  compiled for Windows XP.

* The cpl.dll, rfidtx.dll and rfid.dll files MUST be installed on the
  target system, BUT may be placed in any of the directories outline in
  the [Build Notes] section.

Merge Module:
-------------

* To facilitate projects utilizing this assembly, another related
  'Merge Module' project is included in this solution:
  i.e. CSharp Net Linkage - Build Merge Module.

* The merge module project bundles the output from this project into
  the relative directory [TARGETDIR]\Common\CSharp.

* The merge module project bundles the required native DLLs into the
 relative directory [TARGETDIR]\Common]\clsPacket.

* If the native DLLs included in the merge module do not show up properly,
  you may have to remove and then add them back into the project.  Their
  location on your development system will need to be specified.

Installer:
----------

* To facilitate installation of Linkage.dll and native R1000 Library DLLs
  to known locations, the project CShare NET Linkage - Build Setup File
  is included in this solution.

* The installer project utilizes the output of the Merge Module project
  previously described to determine files to include, and their relative
  target location on the system.

* The default installation directory for the included files are:

  [Program Files]\IMPINJ\Indy\Common\CSharp
  e.g. for the file Linkage.dll and documentation

  [Program Files]\IMPINJ\Indy\Common\Native
  e.g.  for native R1000 Library DLLs:  cpl.dll, rfidtx.dll and rfid.dll

Version Update:
---------------

* Refer to the text file "VersionMe.txt".


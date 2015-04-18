speediff
========

speediff is a C# WinForms app that enables you to quickly compare past Git or SVN revisions for a file. It shows you a diff of two consecutive revisions, and allows you to move forward or back to other pairs of revisions. It preloads all of the revisions, to minimize annoyance when dealing with a slow SVN server.

It is inspired by [SVN Time Lapse View][1], a useful but long-abandoned Java application (which was in turn inspired by Perforce's time-lapse view).

Features:

* Git and SVN support
* Keyboard-friendly navigation
* TortoiseSVN Repo Browser, Log, Blame, and Diff (if installed)
* Custom external app support (examples are in speediff.exe.config)


-Adam Coyne <github@mail2.coyne.nu>

![Screenshot](https://github.com/bhank/speediff/raw/master/speediff-screenshot.png)

[1]: https://code.google.com/p/svn-time-lapse-view/ "SVN Time Lapse View"
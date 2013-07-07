Welcome to ZTOOLS 7/3.1 for Win32 platforms.  This archive contains versions
of the following programs, compiled for Win32 platforms (Win95/98/ME/NT/2000):

check 		- checks story file integrity, same as $verify command. Works on all
        	types except for V1, V2 and early V3 games which have no checksum
        	in the header. Optionally outputs a new story file of the correct
        	length.

        	usage: check story-file-name [new-story-file-name]

infodump 	- dumps header, object list, grammar and dictionary of a given
           	story file. Works on all types. Grammar doesn't work for V6
           	games yet.

           	usage: infodump [options...] story-file-name

pix2gif 	- converts IBM MG1/EG1/EG2 picture files from V6 games to
          	individual GIF files, viewable on most platforms.

          	usage: pix2gif picture-file-name

txd 		- disassembles story files to Z-code assembly language, plus text
      		strings. Works on all games from V1 to V8. Also supports Inform
      		assembler syntax.

      		usage: txd [options...] story-file-name

** NOTE **

These are Win32 console-mode applications and do not have a GUI.  They operate
exactly as the MS-DOS versions and must be run from a command prompt in
Windows 95, 98, ME, NT, or 2000.  If you don't know what to do with these programs or
why you want them, then you probably don't.

Please notify me of any bugs (system crashs, GPFs, etc.), as I usually fix
them (when I'm able) quickly.

Enjoy!

- Kirk Klobe

kirkus@mediaone.net

02 Jun 2001

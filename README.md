# Hardline Dreams - A Matrix Online Server Emulator written in C#
===============================================================

## About Hardline Dreams
We are still here ! 

This is the development git repository for Hardline Dreams (HDS) a Matrix Online Server Emulator.
Our Goal is to get most features working like in the good old MxO Game.

For more information check out our site: http://www.mxoemu.com 

The benefit is that we want to target both combat systems - pre-CR2 and CR2.
As we got a working client in 2014 from Community Member Draxxx (many thanks to him at this point) we want to get both working.

Also we started the analyse for the CR1 Client and figured out some differences (RPC Header Mapping is different, player object has one attribute that was added later etc.)
Currently we handle that with different namespaces but this isnt a clean solution at all.

So we will use different branches for CR2 and pre-CR2 (aka CR1). The Master branch will stay always CR2. 
Well this is an idea - but maybe we change this later. 

## Current State
As we didnt commit a while here this is the current develop version. It is not stable at all.
Currently many features are just working "partically" or just started to implement them.
But this is nearly a state which will be deployed to our "experimental" Server soon.

Partically working features:
- Hyperjump (works but crappy and not really high enough)
- Multi Player support (it "should" work but wasnt tested enough so you should see each other and hopefully you can chat)
- Mission System (still at the beginning)
- Ability loadout change (still buggy if you unload/load more than one ability)
- Ability Vendor at Mara C just for tests (but you can buy items)
- Inventory System (you can wear items but stackable doesnt work currently)
- Friendlist (could be buggy too - not tested)
- Teleporting through hardlines (this should work nearly perfect)
- Opening Doors (for this you need an extra file with the static objects - so currently not working here and also not finished properly)
- Mobs are working partically (they get spawned and auto-move a little bit but still some calculations are wrong - you cannot fight them and they are not attacking you)
  EDIT: you can partically fight them (attack them with Hacker Attacks for example but its still very buggy)
- Vendors are implemented (but selling items doesnt work and buy items doesnt decrease amount of money :))
  Also i parsed all possible vendors from logs but many vendors missing which need be filled later.
  For Fun and that you didnt see a vendor window the first vendor from the CSV is the "default vendor".
- Prepared Crew and Factions Functionality (just prepared - you cann ask a user to join crew but nothing will really happen)

Server Features:
- Entity and View System: Every View has an internal entityId so that we can just spawn view on static and dynmaic obbjects. Well as it is some time ago where i implemented it - i dunno how good it works.
- Network System changed a lot: we have now message Queues for RPC and Object related Messages. And it will be resend until ack (if it works properly :))
- Encryption Library Changed: to move to .NET Framework 4.0 and to compile for 64 Bit i created an encryption interface. So we use a C# Library and a C# implementation.
  The Reason for this is that EngimaLib had to be recompiled for 64 Bit (as it is a C++ Library) but i am not sure if the base libs are able to compile 64 Bit.
  As i dont know if this works well and how good the performance is i created a Encryption Interface and moved the Engima Implemenation there.
  So if it doesnt work we could easily change back.

## The Future
We still continue the development on this project. 
The next goals are:
- hyperjump improvements
- Abilities part 1 (hyperspeed, castable abilities for buffs for example)
- doors with correct values (currently we have some wrong values)
- more and more researching


## Other Files
I commited the debug folder for the reason that there are some resource files inside.
The resource files are contributed by MxOSource (for missions XML files - which are currently unused by hds) and several data files which are contributed by the community and mxoemu.info (Rajkosto).
So special thanks for this resources goes to them. 

If you find files where you see a credit by yourself, feel free to contact us on hardline dreams and i add you here too.

## Activity
If we dont commit here, it doesnt mean that we didnt something on it. We want just to commit some major updates in the future.
Also check out our other repositories for helpful stuff like packets tools etc.


## Development Enviroment Setup
You can use Visual Studio 2017 (which we recommend currently and use it). 
However - Mono Develop should work too.

Rider works again too (2017.2 EAP) - the 2017.1 had some bugs where HDS didnt really start.

## Get the Server running
First you should extract the data.zip file in the "Debug/data" directory (there are 3 CSV Files).
These files holds all static gameobjects for the 3 worlds files (constructs are missing currently).

So you should be able to open doors.
Please Note: they are parsed using a tool we called "Cortana" in the past (before windows 10 exists :)). 
We had some bugs parsing it so some doors position are not correctly currently.
You can checkout the tool here https://github.com/hdneo/cortana-python.git

Second install a MySQL Server somewhere and change the Settings in Debug/Config.xml. 
Import the SQL which is placed in the SQL Folder. 

Start the Server and if everything is right it should take some minutes and end with "I am running :D".

## Thanks & Credits
Just as i forgot i want to give some thanks and credits.
I didnt go deep in detail for what so here it is.
* Rajkosto (mxoemu.info) - many base logic like the encryption, GoProps etc. are based from him.
* Morpheus (hardlinedreams) - Many researching stuff and "cortana". Many thanks mate :)
* The Whole MxO Community - Just for Motivation and support and sure for packet researching too. 

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

## Current State
As we didnt commit a while here this is the current develop version. It is not stable at all.
Currently many features are just working "partically" or just started to implement them.

Partically working features:
- Hyperjump (works but crappy and not really high enough)
- Multi Player support (it "should" work but wasnt tested enough so you should see each other and hopefully you can chat)
- Mission System (still at the beginning)
- Ability loadout change (still buggy if you unload/load more than one ability)
- Ability Vendor at Mara C just for tests (but you can buy items)
- Inventory System (started but not finished - you cannot apply clothing to yourself but you can move and buy things - also stackable items not work properly)
- Friendlist (could be buggy too)
- Teleporting through hardlines (this should work nearly perfect)
- Opening Doors (for this you need an extra file with the static objects - so currently not working here and also not finished properly)
- Mobs are working partically (they get spawned and auto-move a little bit but still some calculations are wrong - you cannot fight them and they are not attacking you)

Server Features:
- Entity and View System: Every View has an internal entityId so that we can just spawn view on static and dynmaic obbjects. Well as it is some time ago where i implemented it - i dunno how good it works.
- Network System changed a lot: we have now message Queues for RPC and Object related Messages. And it will be resend until ack (if it works properly :))

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

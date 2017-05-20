Imports NZag.Core.Tests.Mocks
Imports NZag.Utilities
Imports Xunit

Public Class GameTests

    <Fact()>
    Async Function RunCZech() As Task
        Dim expected =
        <![CDATA[
CZECH: the Comprehensive Z-machine Emulation CHecker, version 0.8
Test numbers appear in [brackets].

print works or you wouldn't be seeing this.

Jumps [2]: jump.je..........jg.......jl.......jz...offsets..
Variables [32]: push/pull..store.load.dec.......inc.......
    dec_chk...........inc_chk.........
Arithmetic ops [70]: add.......sub.......
    mul........div...........mod...........
Logical ops [114]: not....and.....or.....art_shift........log_shift........
Memory [144]: loadw.loadb..storeb..storew...
Subroutines [152]: call_1s.call_2s..call_vs2...call_vs.....ret.
    call_1n.call_2n..call_vn..call_vn2..
    rtrue.rfalse.ret_popped.
    Computed call...
    check_arg_count................
Objects [193]: get_parent....get_sibling.......get_child......jin.......
    test_attr......set_attr....clear_attr....set/clear/test_attr..
    get_next_prop......get_prop_len/get_prop_addr....
    get_prop..........put_prop ..........
    remove..insert.......
    Spec1.0 length-64 props...........
Indirect Opcodes [283]: load..................store.........................
    pull...............inc...............dec...............
    inc_chk...............dec_chk...............
Misc [401]: test...random.verify.piracy.
Header (No tests)
    standard 1.0 
    interpreter 6 A (IBM PC)
    Flags on: 
    Flags off: color, pictures, boldface, italic, fixed-space, sound, timer, transcripting on, fixed-pitch on, redraw pending, using pictures, using undo, using mouse, using colors, using sound, using menus, 
    Screen size: 0x0; in 0x0 units: 0x0
    Default color: current on current



Print opcodes [407]: Tests should look like... '[Test] opcode (stuff): stuff'
print_num (0, 1, -1, 32767,-32768, -1): 0, 1, -1, 32767, -32768, -1
[413] print_char (abcd): abcd
[417] new_line:

There should be an empty line above this line.
print_ret (should have newline after this).
print_addr (Hello.): Hello.

print_paddr (A long string that Inform will put in high memory):
A long string that Inform will put in high memory
Abbreviations (I love 'xyzzy' [two times]): I love 'xyzzy'  I love 'xyzzy'

[424] print_obj (Test Object #1Test Object #2): Test Object #1Test Object #2


Performed 425 tests.
Passed: 406, Failed: 0, Print tests: 19
Didn't crash: hooray!
Last test: quit!
]]>

        Await Test(CZech, expected)
    End Function

    <Fact()>
    Async Function RunZork1() As Task
        Dim expected =
        <![CDATA[
ZORK I: The Great Underground Empire
Copyright (c) 1981, 1982, 1983 Infocom, Inc. All rights reserved.
ZORK is a registered trademark of Infocom, Inc.
Revision 88 / Serial number 840726

West of House
You are standing in an open field west of a white house, with a boarded front door.
There is a small mailbox here.

>N
North of House
You are facing the north side of a white house. There is no door here, and all the windows are boarded up. To the north a narrow path winds through the trees.

>N
Forest Path
This is a path winding through a dimly lit forest. The path heads north-south here. One particularly large tree with some low branches stands at the edge of the path.

>U
Up a Tree
You are about 10 feet above the ground nestled among some large branches. The nearest branch above you is above your reach.
Beside you on the branch is a small bird's nest.
In the bird's nest is a large egg encrusted with precious jewels, apparently scavenged by a childless songbird. The egg is covered with fine gold inlay, and ornamented in lapis lazuli and mother-of-pearl. Unlike most eggs, this one is hinged and closed with a delicate looking clasp. The egg appears extremely fragile.
You hear in the distance the chirping of a song bird.
>GET EGG
Taken.You hear in the distance the chirping of a song bird.
>D
Forest Path

>S
North of House

>E
Behind House
You are behind the white house. A path leads into the forest to the east. In one corner of the house there is a small window which is slightly ajar.

>OPEN WINDOW
With great effort, you open the window far enough to allow entry.

>W
Kitchen
You are in the kitchen of the white house. A table seems to have been used recently for the preparation of food. A passage leads to the west and a dark staircase can be seen leading upward. A dark chimney leads down and to the east is a small window which is open.On the table is an elongated brown sack, smelling of hot peppers.
A bottle is sitting on the table.
The glass bottle contains:  A quantity of water

>GET ALL
brown sack: Taken.glass bottle: Taken.
>W
Living Room
You are in the living room. There is a doorway to the east, a wooden door with strange gothic lettering to the west, which appears to be nailed shut, a trophy case, and a large oriental rug in the center of the room.
Above the trophy case hangs an elvish sword of great antiquity.
A battery-powered brass lantern is on the trophy case.

>OPEN SACK
Opening the brown sack reveals a lunch, and a clove of garlic.
>GET LUNCH AND GARLIC
lunch: Taken.clove of garlic: Taken.
>DROP ALL
clove of garlic: Dropped.lunch: Dropped.glass bottle: Dropped.brown sack: Dropped.jewel-encrusted egg: Dropped.
>GET SWORD AND LAMP
sword: Taken.brass lantern: Taken.
>MOVE RUG
With a great effort, the rug is moved to one side of the room, revealing the dusty cover of a closed trap door.

>OPEN TRAPDOOR
The door reluctantly opens to reveal a rickety staircase descending into darkness.

>OPEN CASE
Opened.
>E
Kitchen

>U
You have moved into a dark place.
It is pitch black. You are likely to be eaten by a grue.

>TURN ON LAMP
The brass lantern is now on.

Attic
This is the attic. The only exit is a stairway leading down.
A large coil of rope is lying in the corner.
On a table is a nasty-looking knife.

>GET ALL
nasty knife: Taken.rope: Taken.
>D
Kitchen

>W
Living Room
There is a jewel-encrusted egg here.
There is a brown sack here.
There is a glass bottle here.
The glass bottle contains:  A quantity of water
A hot pepper sandwich is here.
There is a clove of garlic here.

>DROP KNIFE
Dropped.
>D
The trap door crashes shut, and you hear someone barring it.

Cellar
You are in a dark and damp cellar with a narrow passageway leading north, and a crawlway to the south. On the west is the bottom of a steep metal ramp which is unclimbable.Your sword is glowing with a faint blue glow.

>S
East of Chasm
You are on the east edge of a chasm, the bottom of which cannot be seen. A narrow passage goes north, and the path you are on continues to the east.
Your sword is no longer glowing.

>E
Gallery
This is an art gallery. Most of the paintings have been stolen by vandals with exceptional taste. The vandals left through either the north or west exits.
Fortunately, there is still one chance for you to be a vandal, for on the far wall is a painting of unparalleled beauty.

>GET PAINTING
Taken.
>W
East of Chasm

>N
Cellar
Your sword is glowing with a faint blue glow.

>N
The Troll Room
This is a small room with passages to the east and south and a forbidding hole leading west. Bloodstains and deep scratches (perhaps made by an axe) mar the walls.
A nasty-looking troll, brandishing a bloody axe, blocks all passages out of the room.
Your sword has begun to glow very brightly.
The troll's axe barely misses your ear.

>KILL TROLL
(with the sword)
The troll's weapon is knocked to the floor, leaving him unarmed.
The troll, angered and humiliated, recovers his weapon. He appears to have an axe to grind with you.
>KILL TROLL
(with the sword)
You charge, but the troll jumps nimbly aside.
The troll swings his axe, but it misses.

>KILL TROLL
(with the sword)
You charge, but the troll jumps nimbly aside.
The troll's axe barely misses your ear.

>KILL TROLL
(with the sword)
The troll is staggered, and drops to his knees.
The troll slowly regains his feet.

>KILL TROLL
(with the sword)
You charge, but the troll jumps nimbly aside.
The troll hits you with a glancing blow, and you are momentarily stunned.

>KILL TROLL
(with the sword)
You are still recovering from that last blow, so your attack is ineffective.
The troll swings his axe, but it misses.

>KILL TROLL
(with the sword)
A furious exchange, and the troll is knocked out!

>KILL TROLL
(with the sword)
The unarmed troll cannot defend himself: He dies.
Almost as soon as the troll breathes his last breath, a cloud of sinister black fog envelops him, and when the fog lifts, the carcass has disappeared.
Your sword is no longer glowing.

>DROP SWORD
Dropped.Someone carrying a large bag is casually leaning against one of the walls here. He does not speak, but it is clear from his aspect that the bag will be taken only over his dead body.

>E
East-West Passage
This is a narrow east-west passageway. There is a narrow stairway leading down at the north end of the room.

>E
Round Room
This is a circular stone room with passages in all directions. Several of them have unfortunately been blocked by cave-ins.

>SE
Engravings Cave
You have entered a low cave with passages leading northwest and east.
There are old engravings on the walls here.

>E
Dome Room
You are at the periphery of a large dome, which forms the ceiling of another room below. Protecting you from a precipitous drop is a wooden railing which circles the dome.

>TIE ROPE TO RAILING
The rope drops over the side and comes within ten feet of the floor.

>D
Torch Room
This is a large room with a prominent doorway leading to a down staircase. Above you is a large dome. Up around the edge of the dome (20 feet up) is a wooden railing. In the center of the room sits a white marble pedestal.
A piece of rope descends from the railing above, ending some five feet above your head.Sitting on the pedestal is a flaming torch, made of ivory.

>S
Temple
This is the north end of a large temple. On the east wall is an ancient inscription, probably a prayer in a long-forgotten language. Below the prayer is a staircase leading down. The west wall is solid granite. The exit to the north end of the room is through huge marble pillars.
There is a brass bell here.

>E
Egyptian Room
This is a room which looks like an Egyptian tomb. There is an ascending staircase to the west.
The solid-gold coffin used for the burial of Ramses II is here.

>GET COFFIN
Taken.
>W
Temple
There is a brass bell here.

>S
Altar
This is the south end of a large temple. In front of you is what appears to be an altar. In one corner is a small hole in the floor which leads into darkness. You probably could not get back up it.
On the two ends of the altar are burning candles.
On the altar is a large black book, open to page 569.

>PRAY
Forest
This is a forest, with trees in all directions. To the east, there appears to be sunlight.
You hear in the distance the chirping of a song bird.
>S
Forest
This is a dimly lit forest, with large trees all around.

>N
Clearing
You are in a small clearing in a well marked forest path that extends to the east and west.

>W
Behind House

>W
Kitchen

>W
Living Room
There is a nasty knife here.
There is a jewel-encrusted egg here.
There is a brown sack here.
There is a glass bottle here.
The glass bottle contains:  A quantity of water
A hot pepper sandwich is here.
There is a clove of garlic here.

>OPEN COFFIN
The gold coffin opens.
A sceptre, possibly that of ancient Egypt itself, is in the coffin. The sceptre is ornamented with colored enamel, and tapers to a sharp point.

>GET SCEPTRE
Taken.
>PUT COFFIN AND PAINTING IN CASE
gold coffin: Done.painting: Done.
>E
Kitchen

>E
Behind House

>E
Clearing

>E
Canyon View
You are at the top of the Great Canyon on its west wall. From here there is a marvelous view of the canyon and parts of the Frigid River upstream. Across the canyon, the walls of the White Cliffs join the mighty ramparts of the Flathead Mountains to the east. Following the Canyon upstream to the north, Aragain Falls may be seen, complete with rainbow. The mighty Frigid River flows out from a great dark cavern. To the west and south can be seen an immense forest, stretching for miles around. A path leads northwest. It is possible to climb down into the canyon from here.

>D
Rocky Ledge
You are on a ledge about halfway up the wall of the river canyon. You can see from here that the main flow from Aragain Falls twists along a passage which it is impossible for you to enter. Below you is the canyon bottom. Above you is more cliff, which appears climbable.

>D
Canyon Bottom
You are beneath the walls of the river canyon which may be climbable here. The lesser part of the runoff of Aragain Falls flows by below. To the north is a narrow path.

>N
End of Rainbow
You are on a small, rocky beach on the continuation of the Frigid River past the Falls. The beach is narrow due to the presence of the White Cliffs. The river canyon opens here and sunlight shines in from above. A rainbow crosses over the falls to the east and a narrow path continues to the southwest.

>WAVE SCEPTRE
Suddenly, the rainbow appears to become solid and, I venture, walkable (I think the giveaway was the stairs and bannister).
A shimmering pot of gold appears at the end of the rainbow.

>E
On the Rainbow
You are on top of a rainbow (I bet you never thought you would walk on a rainbow), with a magnificent view of the Falls. The rainbow travels east-west here.

>E
Aragain Falls
You are at the top of Aragain Falls, an enormous waterfall with a drop of about 450 feet. The only path here is on the north end.
A solid rainbow spans the falls.

>N
Shore
You are on the east shore of the river. The water here seems somewhat treacherous. A path travels from north to south here, the south end quickly turning around a sharp corner.

>N
Sandy Beach
You are on a large sandy beach on the east shore of the river, which is flowing quickly by. A path runs beside the river to the south here, and a passage is partially buried in sand to the northeast.
There is a shovel here.

>GET SHOVEL
Taken.
>NE
Sandy Cave
This is a sand-filled cave whose exit is to the southwest.

>DIG SAND
(with the shovel)
You seem to be digging a hole here.

>AGAIN
The hole is getting deeper, but that's about it.

>AGAIN
You are surrounded by a wall of sand on all sides.

>AGAIN
You can see a scarab here in the sand.

>DROP SHOVEL
Dropped.
>GET SCARAB
Taken.
>SW
Sandy Beach

>S
Shore

>S
Aragain Falls

>W
On the Rainbow

>W
End of Rainbow
At the end of the rainbow is a pot of gold.

>GET POT
Taken.
>SW
Canyon Bottom

>U
Rocky Ledge

>U
Canyon View

>NW
Clearing

>W
Behind House

>W
Kitchen

>W
Living Room
There is a nasty knife here.
There is a jewel-encrusted egg here.
There is a brown sack here.
There is a glass bottle here.
The glass bottle contains:  A quantity of water
A hot pepper sandwich is here.
There is a clove of garlic here.
Your collection of treasures consists of:    A painting
    A gold coffin

>PUT ALL BUT LAMP IN CASE
pot of gold: Done.beautiful jeweled scarab: Done.sceptre: Done.
>GET ALL BUT SACK AND GARLIC
nasty knife: Taken.jewel-encrusted egg: Taken.glass bottle: Taken.lunch: Taken.trophy case: The trophy case is securely fastened to the wall.carpet: The rug is extremely heavy and cannot be carried.
>OPEN TRAPDOOR
The door reluctantly opens to reveal a rickety staircase descending into darkness.

>D
Cellar

>N
The Troll Room
There is a sword here.
There is a bloody axe here.

>W
Maze
This is part of a maze of twisty little passages, all alike.

>S
Maze
This is part of a maze of twisty little passages, all alike.

>E
Maze
This is part of a maze of twisty little passages, all alike.

>U
Maze
This is part of a maze of twisty little passages, all alike. A skeleton, probably the remains of a luckless adventurer, lies here.
Beside the skeleton is a rusty knife.
The deceased adventurer's useless lantern is here.
There is a skeleton key here.
An old leather bag, bulging with coins, is here.
A seedy-looking individual with a large bag just wandered through the room. On the way through, he quietly abstracted some valuables from the room and from your possession, mumbling something about "Doing unto others before..."

>GET BAG AND KEY
large bag: The bag will be taken over his dead body.skeleton key: Taken.
>SW
Maze
This is part of a maze of twisty little passages, all alike.

>E
Maze
This is part of a maze of twisty little passages, all alike.

>S
Maze
This is part of a maze of twisty little passages, all alike.

>SE
Cyclops Room
This room has an exit on the northwest, and a staircase leading up.
A cyclops, who looks prepared to eat horses (much less mere adventurers), blocks the staircase. From his state of health, and the bloodstains on the walls, you gather that he is not very friendly, though he likes people.
>GIVE LUNCH AND WATER TO CYCLOPS
lunch: The cyclops says "Mmm Mmm. I love hot peppers! But oh, could I use a drink. Perhaps I could drink the blood of that thing."  From the gleam in his eye, it could be surmised that you are "that thing".
quantity of water: The cyclops takes the bottle, checks that it's open, and drinks the water. A moment later, he lets out a yawn that nearly blows you over, and then falls fast asleep (what did you put in that drink, anyway?).

>DROP BOTTLE
You don't have the glass bottle.

>U
You hear a scream of anguish as you violate the robber's hideaway. Using passages unknown to you, he rushes to its defense.
The thief gestures mysteriously, and the treasures in the room suddenly vanish.

Treasure Room
This is a large room, whose east wall is solid granite. A number of discarded bags, which crumble at your touch, are scattered about on the floor. There is an exit down a staircase.
There is a suspicious-looking individual, holding a large bag, leaning against one wall. He is armed with a deadly stiletto.
There is a silver chalice, intricately engraved, here.
The thief stabs nonchalantly with his stiletto and misses.

>GIVE EGG TO THIEF
The thief is taken aback by your unexpected generosity, but accepts the jewel-encrusted egg and stops to admire its beauty.
>KILL THIEF
(with the nasty knife)
Clang! Crash! The thief parries.
A long, theatrical slash. You catch it on your nasty knife, but the thief twists his knife, and the nasty knife goes flying.

>GET KNIFE
Taken.A quick thrust pinks your left arm, and blood starts to trickle down.

>KILL THIEF
(with the nasty knife)
The thief's weapon is knocked to the floor, leaving him unarmed.
The robber, somewhat surprised at this turn of events, nimbly retrieves his stiletto.

>KILL THIEF
(with the nasty knife)
Your nasty knife pinks the thief on the wrist, but it's not serious.
The thief stabs nonchalantly with his stiletto and misses.

>KILL THIEF
(with the nasty knife)
The thief is battered into unconsciousness.

>KILL THIEF
(with the nasty knife)
The unarmed thief cannot defend himself: He dies.
Almost as soon as the thief breathes his last breath, a cloud of sinister black fog envelops him, and when the fog lifts, the carcass has disappeared.
As the thief dies, the power of his magic decreases, and his treasures reappear:
  A leather bag of coins
  A jewel-encrusted egg, with a golden clockwork canary
  A stiletto
The chalice is now safe to take.

>GET ALL BUT STILETTO
leather bag of coins: Taken.jewel-encrusted egg: Taken.chalice: Taken.
>DROP KNIFE
Dropped.
>D
Cyclops Room
There is a glass bottle here.

>NW
Maze
This is part of a maze of twisty little passages, all alike.

>S
Maze
This is part of a maze of twisty little passages, all alike.

>W
Maze
This is part of a maze of twisty little passages, all alike.

>U
Maze
This is part of a maze of twisty little passages, all alike.

>D
You won't be able to get back up to the tunnel you are going through when it gets to the next room.

Maze
This is part of a maze of twisty little passages, all alike.

>NE
Grating Room
You are in a small room near the maze. There are twisty passages in the immediate vicinity.
Above you is a grating locked with a skull-and-crossbones lock.

>UNLOCK GRATE
(with the skeleton key)
The grate is unlocked.
>DROP KEY
Dropped.
>OPEN GRATE
The grating opens to reveal trees above you.
A pile of leaves falls onto your head and to the ground.

>U
Clearing
You are in a clearing, with a forest surrounding you on all sides. A path leads south.
There is an open grating, descending into darkness.

>S
Forest Path

>WIND UP CANARY
The canary chirps, slightly off-key, an aria from a forgotten opera. From out of the greenery flies a lovely songbird. It perches on a limb just over your head and opens its beak to sing. As it does so a beautiful brass bauble drops from its mouth, bounces off the top of your head, and lands glimmering in the grass. As the canary winds down, the songbird flies away.
You hear in the distance the chirping of a song bird.
>GET BAUBLE
Taken.
>S
North of House

>E
Behind House

>W
Kitchen

>W
Living Room
There is a brown sack here.
There is a clove of garlic here.
Your collection of treasures consists of:    A sceptre
    A beautiful jeweled scarab
    A pot of gold
    A painting
    A gold coffin

>GET CANARY
Taken.
>PUT ALL BUT LAMP IN CASE
golden clockwork canary: Done.beautiful brass bauble: Done.chalice: Done.jewel-encrusted egg: Done.leather bag of coins: Done.
>GET GARLIC
Taken.
>D
Cellar

>N
The Troll Room
There is a sword here.
There is a bloody axe here.

>E
East-West Passage

>N
Chasm
A chasm runs southwest to northeast and the path follows it. You are on the south side of the chasm, where a crack opens into a passage.

>NE
Reservoir South
You are in a long room on the south shore of a large lake, far too deep and wide for crossing.
There is a path along the stream to the east or west, a steep pathway climbing southwest along the edge of a chasm, and a path leading into a canyon to the southeast.
>E
Dam
You are standing on the top of the Flood Control Dam #3, which was quite a tourist attraction in times far distant. There are paths to the north, south, and west, and a scramble down.
The sluice gates on the dam are closed. Behind the dam, there can be seen a wide reservoir. Water is pouring over the top of the now abandoned dam.
There is a control panel here, on which a large metal bolt is mounted. Directly above the bolt is a small green plastic bubble.
>N
Dam Lobby
This room appears to have been the waiting room for groups touring the dam. There are open doorways here to the north and east marked "Private", and there is a path leading south over the top of the dam.
Some guidebooks entitled "Flood Control Dam #3" are on the reception desk.
There is a matchbook whose cover says "Visit Beautiful FCD#3" here.

>GET MATCHBOOK
Taken.
>E
Maintenance Room
This is what appears to have been the maintenance room for Flood Control Dam #3. Apparently, this room has been ransacked recently, for most of the valuable equipment is gone. On the wall in front of you is a group of buttons colored blue, yellow, brown, and red. There are doorways to the west and south.
There is a group of tool chests here.
There is a wrench here.
There is an object which looks like a tube of toothpaste here.
There is a screwdriver here.

>PUSH YELLOW
Click.
>GET WRENCH AND SCREWDRIVER
wrench: Taken.screwdriver: Taken.
>W
Dam Lobby
Some guidebooks entitled "Flood Control Dam #3" are on the reception desk.

>S
Dam
You are standing on the top of the Flood Control Dam #3, which was quite a tourist attraction in times far distant. There are paths to the north, south, and west, and a scramble down.
The sluice gates on the dam are closed. Behind the dam, there can be seen a wide reservoir. Water is pouring over the top of the now abandoned dam.
There is a control panel here, on which a large metal bolt is mounted. Directly above the bolt is a small green plastic bubble which is glowing serenely.
>DROP GARLIC AND SCREWDRIVER
clove of garlic: Dropped.screwdriver: Dropped.
>TURN BOLT WITH WRENCH
The sluice gates open and water pours through the dam.

>DROP WRENCH
Dropped.
>E
Dam Base
You are at the base of Flood Control Dam #3, which looms above you and to the north. The river Frigid is flowing by here. Along the river are the White Cliffs which seem to form giant walls stretching from north to south along the shores of the river as it winds its way downstream.
There is a folded pile of plastic here which has a small valve attached.

>GET PLASTIC
Taken.
>N
Dam
There is a wrench here.
There is a screwdriver here.
There is a clove of garlic here.

>DROP PLASTIC
Dropped.
>S
Deep Canyon
You are on the south edge of a deep canyon. Passages lead off to the east, northwest and southwest. A stairway leads down. You can hear a loud roaring sound, like that of rushing water, from below.

>SW
North-South Passage
This is a high north-south passage, which forks to the northeast.

>S
Round Room

>E
Loud Room
This is a large room with a ceiling which cannot be detected from the ground. There is a narrow passage from east to west and a stone stairway leading upward. The room is deafeningly loud with an undetermined rushing sound. The sound seems to reverberate from all of the walls, making it difficult even to think.
On the ground is a large platinum bar.

>ECHO
The acoustics of the room change subtly.

Loud Room
On the ground is a large platinum bar.

>GET BAR
Taken.
>W
Round Room

>SE
Engravings Cave
There are old engravings on the walls here.

>E
Dome Room

>D
Torch Room
Sitting on the pedestal is a flaming torch, made of ivory.

>TURN OFF LAMP
The brass lantern is now off.

>GET TORCH
Taken.
>S
Temple
There is a brass bell here.

>GET BELL
Taken.
>S
Altar
On the two ends of the altar are burning candles.
On the altar is a large black book, open to page 569.

>GET ALL
black book: Taken.pair of candles: Taken.
>D
Cave
This is a tiny cave with entrances west and north, and a dark, forbidding staircase leading down.
A gust of wind blows out your candles!

>D
Entrance to Hades
You are outside a large gateway, on which is inscribed

  Abandon every hope all ye who enter here!

The gate is open; through it you can see a desolation, with a pile of mangled bodies in one corner. Thousands of voices, lamenting some hideous fate, can be heard.
The way through the gate is barred by evil spirits, who jeer at your attempts to pass.
>RING BELL
The bell suddenly becomes red hot and falls to the ground. The wraiths, as if paralyzed, stop their jeering and slowly turn to face you. On their ashen faces, the expression of a long-forgotten terror takes shape.
In your confusion, the candles drop to the ground (and they are out).

>GET CANDLES
Taken.
>LIGHT MATCH
One of the matches starts to burn.

>LIGHT CANDLES
(with the match)
The candles are lit.
The flames flicker wildly and appear to dance. The earth beneath your feet trembles, and your legs nearly buckle beneath you. The spirits cower at your unearthly power.
The match has gone out.

>READ BOOK
Each word of the prayer reverberates through the hall in a deafening confusion. As the last word fades, a voice, loud and commanding, speaks: "Begone, fiends!" A heart-stopping scream fills the cavern, and the spirits, sensing a greater power, flee through the walls.

>S
Land of the Dead
You have entered the Land of the Living Dead. Thousands of lost souls can be heard weeping and moaning. In the corner are stacked the remains of dozens of previous adventurers less fortunate than yourself. A passage exits to the north.
Lying in one corner of the room is a beautifully carved crystal skull. It appears to be grinning at you rather nastily.

>GET SKULL
Taken.
>N
Entrance to Hades
On the ground is a red hot bell.

>U
Cave
A gust of wind blows out your candles!

>N
Mirror Room
You are in a large square room with tall ceilings. On the south wall is an enormous mirror which fills the entire wall. There are exits on the other three sides of the room.

>N
Narrow Passage
This is a long and narrow corridor where a long north-south passageway briefly narrows even further.

>N
Round Room

>W
East-West Passage

>W
The Troll Room
There is a sword here.
There is a bloody axe here.

>S
Cellar

>U
Living Room
There is a brown sack here.
Your collection of treasures consists of:    A leather bag of coins
    A jewel-encrusted egg
    A chalice
    A beautiful brass bauble
    A golden clockwork canary
    A sceptre
    A beautiful jeweled scarab
    A pot of gold
    A painting
    A gold coffin

>PUT SKULL AND BAR IN CASE
crystal skull: Done.platinum bar: Done.
>D
Cellar

>N
The Troll Room
There is a sword here.
There is a bloody axe here.

>E
East-West Passage

>N
Chasm

>NE
Reservoir South
You are in a long room, to the north of which was formerly a lake. However, with the water level lowered, there is merely a wide stream running through the center of the room.
There is a path along the stream to the east or west, a steep pathway climbing southwest along the edge of a chasm, and a path leading into a canyon to the southeast.
>E
Dam
There is a folded pile of plastic here which has a small valve attached.
There is a wrench here.
There is a screwdriver here.
There is a clove of garlic here.

>GET GARLIC AND SCREWDRIVER
clove of garlic: Taken.screwdriver: Taken.
>W
Reservoir South

>N
Reservoir
You are on what used to be a large lake, but which is now a large mud pile. There are "shores" to the north and south.
Lying half buried in the mud is an old trunk, bulging with jewels.

>DROP ALL BUT TORCH
screwdriver: Dropped.clove of garlic: Dropped.pair of candles: Dropped.black book: Dropped.matchbook: Dropped.brass lantern: Dropped.
>GET TRUNK
Taken.
>N
Reservoir North
You are in a large cavernous room, the south of which was formerly a lake. However, with the water level lowered, there is merely a wide stream running through there.
There is a slimy stairway leaving the room to the north.There is a hand-held air pump here.

>N
Atlantis Room
This is an ancient room, long under water. There is an exit to the south and a staircase leading up.
On the shore lies Poseidon's own crystal trident.

>GET TRIDENT
Taken.
>S
Reservoir North
There is a hand-held air pump here.

>S
Reservoir
There is a brass lantern (battery-powered) here.
There is a matchbook whose cover says "Visit Beautiful FCD#3" here.
There is a black book here.
There is a pair of candles here.
There is a clove of garlic here.
There is a screwdriver here.

>S
Reservoir South

>SW
Chasm

>SW
East-West Passage

>W
The Troll Room
There is a sword here.
There is a bloody axe here.

>S
Cellar

>U
Living Room
There is a brown sack here.
Your collection of treasures consists of:    A platinum bar
    A crystal skull
    A leather bag of coins
    A jewel-encrusted egg
    A chalice
    A beautiful brass bauble
    A golden clockwork canary
    A sceptre
    A beautiful jeweled scarab
    A pot of gold
    A painting
    A gold coffin

>PUT TRUNK AND TRIDENT IN CASE
trunk of jewels: Done.crystal trident: Done.
>D
Cellar

>N
The Troll Room
There is a sword here.
There is a bloody axe here.

>E
East-West Passage

>N
Chasm

>NE
Reservoir South

>N
Reservoir
There is a brass lantern (battery-powered) here.
There is a matchbook whose cover says "Visit Beautiful FCD#3" here.
There is a black book here.
There is a pair of candles here.
There is a clove of garlic here.
There is a screwdriver here.

>GET ALL
brass lantern: Taken.matchbook: Taken.black book: Taken.pair of candles: Taken.clove of garlic: Taken.screwdriver: Taken.
>N
Reservoir North
There is a hand-held air pump here.

>N
Atlantis Room

>U
Cave
This is a tiny cave with entrances west and north, and a staircase leading down.

>N
Mirror Room
You are in a large square room with tall ceilings. On the south wall is an enormous mirror which fills the entire wall. There are exits on the other three sides of the room.

>N
Cold Passage
This is a cold and damp corridor where a long east-west passageway turns into a southward path.

>W
Slide Room
This is a small chamber, which appears to have been part of a coal mine. On the south wall of the chamber the letters "Granite Wall" are etched in the rock. To the east is a long passage, and there is a steep metal slide twisting downward. To the north is a small opening.

>N
Mine Entrance
You are standing at the entrance of what might have been a coal mine. The shaft enters the west wall, and there is another exit on the south end of the room.

>W
Squeaky Room
You are in a small room. Strange squeaky sounds may be heard coming from the passage at the north end. You may also escape to the east.

>DROP TORCH
Dropped.
>TURN ON LAMP
The brass lantern is now on.

>N
Bat Room
You are in a small room which has doors only to the east and south.In the corner of the room on the ceiling is a large vampire bat who is obviously deranged and holding his nose.There is an exquisite jade figurine here.

>GET FIGURINE
Taken.
>S
Squeaky Room
There is a torch here (providing light).

>DROP FIGURINE
Dropped.
>N
Bat Room
In the corner of the room on the ceiling is a large vampire bat who is obviously deranged and holding his nose.
>E
Shaft Room
This is a large room, in the middle of which is a small shaft descending through the floor into darkness below. To the west and the north are exits from this room. Constructed over the top of the shaft is a metal framework to which a heavy iron chain is attached.
At the end of the chain is a basket.

>DROP ALL BUT LAMP AND GARLIC
screwdriver: Dropped.pair of candles: Dropped.black book: Dropped.matchbook: Dropped.
>N
Smelly Room
This is a small non-descript room. However, from the direction of a small descending staircase a foul odor can be detected. To the south is a narrow tunnel.

>D
Gas Room
This is a small room which smells strongly of coal gas. There is a short climb up some stairs and a narrow tunnel leading east.
There is a sapphire-encrusted bracelet here.

>E
Coal Mine
This is a non-descript part of a coal mine.

>NE
Coal Mine
This is a non-descript part of a coal mine.

>SE
Coal Mine
This is a non-descript part of a coal mine.

>SW
Coal Mine
This is a non-descript part of a coal mine.

>D
Ladder Top
This is a very small room. In the corner is a rickety wooden ladder, leading downward. It might be safe to descend. There is also a staircase leading upward.

>D
Ladder Bottom
This is a rather wide room. On one side is the bottom of a narrow wooden ladder. To the west and the south are passages leaving the room.

>S
Dead End
You have come to a dead end in the mine.
There is a small pile of coal here.

>GET COAL
Taken.
>N
Ladder Bottom

>U
Ladder Top

>U
Coal Mine

>N
Coal Mine

>E
Coal Mine

>S
Coal Mine

>N
Gas Room
There is a sapphire-encrusted bracelet here.

>U
Smelly Room

>S
Shaft Room
There is a matchbook whose cover says "Visit Beautiful FCD#3" here.
There is a black book here.
There is a pair of candles here.
There is a screwdriver here.
At the end of the chain is a basket.

>GET ALL
matchbook: Taken.black book: Taken.pair of candles: Taken.screwdriver: Taken.basket: The cage is securely fastened to the iron chain.
>PUT COAL AND SCREWDRIVER IN BASKET
small pile of coal: Done.screwdriver: Done.
>LIGHT MATCH
One of the matches starts to burn.

>LIGHT CANDLES WITH MATCH
The candles are lit.
The match has gone out.

>PUT CANDLES IN BASKET
Done.
>LOWER BASKET
The basket is lowered to the bottom of the shaft.

>DROP MATCHBOOK
Dropped.
>N
Smelly Room

>D
Gas Room
There is a sapphire-encrusted bracelet here.

>E
Coal Mine

>NE
Coal Mine

>SE
Coal Mine

>SW
Coal Mine

>D
Ladder Top

>D
Ladder Bottom

>W
Timber Room
This is a long and narrow passage, which is cluttered with broken timbers. A wide passage comes from the east and turns at the west end of the room into a very narrow passageway. From the west comes a strong draft.
There is a broken timber here.

>DROP ALL
black book: Dropped.clove of garlic: Dropped.brass lantern: Dropped.
>W
Drafty Room
This is a small drafty room in which is the bottom of a long shaft. To the south is a passageway and to the east a very narrow passage. In the shaft can be seen a heavy iron chain.
At the end of the chain is a basket.
The basket contains:  A pair of candles (providing light)
  A screwdriver
  A small pile of coal

>GET COAL AND CANDLES AND SCREWDRIVER
small pile of coal: Taken.pair of candles: Taken.screwdriver: Taken.
>S
Machine Room
This is a large, cold room whose sole exit is to the north. In one corner there is a machine which is reminiscent of a clothes dryer. On its face is a switch which is labelled "START". The switch does not appear to be manipulable by any human hand (unless the fingers are about 1/16 by 1/4 inch). On the front of the machine is a large lid, which is closed.

>OPEN LID
The lid opens.

>PUT COAL IN MACHINE
Done.
>CLOSE LID
The lid closes.

>TURN SWITCH WITH SCREWDRIVER
The machine comes to life (figuratively) with a dazzling display of colored lights and bizarre noises. After a few moments, the excitement abates.

>DROP SCREWDRIVER
Dropped.
>OPEN LID
The lid opens, revealing a huge diamond.

>GET DIAMOND
Taken.
>N
Drafty Room
At the end of the chain is a basket.

>PUT DIAMOND IN BASKET
Done.
>DROP CANDLES
Dropped.
>E
Timber Room
There is a brass lantern (battery-powered) here.
There is a clove of garlic here.
There is a black book here.
There is a broken timber here.

>GET ALL BUT TIMBER
brass lantern: Taken.clove of garlic: Taken.black book: Taken.
>E
Ladder Bottom

>U
Ladder Top

>U
Coal Mine

>N
Coal Mine

>E
Coal Mine

>S
Coal Mine

>N
Gas Room
There is a sapphire-encrusted bracelet here.

>GET SAPPHIRE
Taken.
>U
Smelly Room

>S
Shaft Room
There is a matchbook whose cover says "Visit Beautiful FCD#3" here.
From the chain is suspended a basket.

>RAISE BASKET
The basket is raised to the top of the shaft.
>GET DIAMOND
Taken.
>W
Bat Room
In the corner of the room on the ceiling is a large vampire bat who is obviously deranged and holding his nose.
>S
Squeaky Room
There is an exquisite jade figurine here.
There is a torch here (providing light).

>GET ALL
jade figurine: Taken.torch: Taken.
>E
Mine Entrance

>S
Slide Room

>D
Cellar

>U
Living Room
There is a brown sack here.
Your collection of treasures consists of:    A crystal trident
    A trunk of jewels
    A platinum bar
    A crystal skull
    A leather bag of coins
    A jewel-encrusted egg
    A chalice
    A beautiful brass bauble
    A golden clockwork canary
    A sceptre
    A beautiful jeweled scarab
    A pot of gold
    A painting
    A gold coffin

>DROP GARLIC
Dropped.
>PUT ALL BUT LAMP IN CASE
torch: Done.jade figurine: Done.huge diamond: Done.sapphire-encrusted bracelet: Done.black book: Done.
>D
Cellar

>N
The Troll Room
There is a sword here.
There is a bloody axe here.

>E
East-West Passage

>N
Chasm

>NE
Reservoir South

>N
Reservoir

>N
Reservoir North
There is a hand-held air pump here.

>GET PUMP
Taken.
>S
Reservoir

>S
Reservoir South

>E
Dam
There is a folded pile of plastic here which has a small valve attached.
There is a wrench here.

>GET PLASTIC
Taken.
>S
Deep Canyon
You are on the south edge of a deep canyon. Passages lead off to the east, northwest and southwest. A stairway leads down. You can hear the sound of flowing water from below.

>D
Loud Room

>E
Damp Cave
This cave has exits to the west and east, and narrows to a crack toward the south. The earth is particularly damp here.

>E
White Cliffs Beach
You are on a narrow strip of beach which runs along the base of the White Cliffs. There is a narrow path heading south along the Cliffs and a tight passage leading west into the cliffs themselves.

>S
White Cliffs Beach
You are on a rocky, narrow strip of beach beside the Cliffs. A narrow path leads north along the shore.

>DROP PLASTIC
Dropped.
>INFLATE PLASTIC
(with the hand-held air pump)
The boat inflates and appears seaworthy.
A tan label is lying inside the boat.

>DROP PUMP
Dropped.
>ENTER BOAT
You are now in the magic boat.

>LAUNCH
(magic boat)
Frigid River, in the magic boat
The river is running faster here and the sound ahead appears to be that of rushing water. On the east shore is a sandy beach. A small area of beach can also be seen below the cliffs on the west shore.
There is a red buoy here (probably a warning).
The magic boat contains:  A tan label

>GET BUOY
Taken.The flow of the river carries you downstream.

Frigid River, in the magic boat
The sound of rushing water is nearly unbearable here. On the east shore is a large landing area.
The magic boat contains:  A tan label

>LAND
The magic boat comes to a rest on the shore.

Shore, in the magic boat
The magic boat contains:  A tan label

>GET OUT OF BOAT
You are on your own feet again.

>S
Aragain Falls

>W
On the Rainbow

>W
End of Rainbow

>SW
Canyon Bottom

>U
Rocky Ledge

>U
Canyon View

>NW
Clearing

>W
Behind House

>W
Kitchen

>W
Living Room
There is a clove of garlic here.
There is a brown sack here.
Your collection of treasures consists of:    A black book
    A sapphire-encrusted bracelet
    A huge diamond
    A jade figurine
    A torch (providing light)
    A crystal trident
    A trunk of jewels
    A platinum bar
    A crystal skull
    A leather bag of coins
    A jewel-encrusted egg
    A chalice
    A beautiful brass bauble
    A golden clockwork canary
    A sceptre
    A beautiful jeweled scarab
    A pot of gold
    A painting
    A gold coffin

>OPEN BUOY
Opening the red buoy reveals a large emerald.
>PUT EMERALD IN CASE
Done.An almost inaudible voice whispers in your ear, "Look to your treasures for the final secret."
>E
Kitchen

>E
Behind House

>N
North of House

>W
West of House
You are standing in an open field west of a white house, with a boarded front door. A secret path leads southwest into the forest.
There is a small mailbox here.

>SW
Stone Barrow
You are standing in front of a massive barrow of stone. In the east face is a huge stone door which is open. You cannot see into the dark of the tomb.

>W
Inside the Barrow
As you enter the barrow, the door closes inexorably behind you. Around you it is dark, but ahead is an enormous cavern, brightly lit. Through its center runs a wide stream. Spanning the stream is a small wooden footbridge, and beyond a path leads into a dark tunnel. Above the bridge, floating in the air, is a large sign. It reads:  All ye who stand before this bridge have completed a great and perilous adventure which has tested your wit and courage. You have mastered the first part of the ZORK trilogy. Those who pass over this bridge must be prepared to undertake an even greater adventure that will severely test your skill and bravery!

The ZORK trilogy continues with "ZORK II: The Wizard of Frobozz" and is completed in "ZORK III: The Dungeon Master."
Your score is 350 (total of 350 points), in 355 moves.
This gives you the rank of Master Adventurer.
]]>

        Dim script =
        <![CDATA[
N
N
U
GET EGG
D
S
E
OPEN WINDOW
W
GET ALL
W
OPEN SACK
GET LUNCH AND GARLIC
DROP ALL
GET SWORD AND LAMP
MOVE RUG
OPEN TRAPDOOR
OPEN CASE
E
U
TURN ON LAMP
GET ALL
D
W
DROP KNIFE
D
S
E
GET PAINTING
W
N
N
KILL TROLL
KILL TROLL
KILL TROLL
KILL TROLL
KILL TROLL
KILL TROLL
KILL TROLL
KILL TROLL
DROP SWORD
E
E
SE
E
TIE ROPE TO RAILING
D
S
E
GET COFFIN
W
S
PRAY
S
N
W
W
W
OPEN COFFIN
GET SCEPTRE
PUT COFFIN AND PAINTING IN CASE
E
E
E
E
D
D
N
WAVE SCEPTRE
E
E
N
N
GET SHOVEL
NE
DIG SAND
AGAIN
AGAIN
AGAIN
DROP SHOVEL
GET SCARAB
SW
S
S
W
W
GET POT
SW
U
U
NW
W
W
W
PUT ALL BUT LAMP IN CASE
GET ALL BUT SACK AND GARLIC
OPEN TRAPDOOR
D
N
W
S
E
U
GET BAG AND KEY
SW
E
S
SE
GIVE LUNCH AND WATER TO CYCLOPS
DROP BOTTLE
U
GIVE EGG TO THIEF
KILL THIEF
GET KNIFE
KILL THIEF
KILL THIEF
KILL THIEF
KILL THIEF
GET ALL BUT STILETTO
DROP KNIFE
D
NW
S
W
U
D
NE
UNLOCK GRATE
DROP KEY
OPEN GRATE
U
S
WIND UP CANARY
GET BAUBLE
S
E
W
W
GET CANARY
PUT ALL BUT LAMP IN CASE
GET GARLIC
D
N
E
N
NE
E
N
GET MATCHBOOK
E
PUSH YELLOW
GET WRENCH AND SCREWDRIVER
W
S
DROP GARLIC AND SCREWDRIVER
TURN BOLT WITH WRENCH
DROP WRENCH
E
GET PLASTIC
N
DROP PLASTIC
S
SW
S
E
ECHO
GET BAR
W
SE
E
D
TURN OFF LAMP
GET TORCH
S
GET BELL
S
GET ALL
D
D
RING BELL
GET CANDLES
LIGHT MATCH
LIGHT CANDLES
READ BOOK
S
GET SKULL
N
U
N
N
N
W
W
S
U
PUT SKULL AND BAR IN CASE
D
N
E
N
NE
E
GET GARLIC AND SCREWDRIVER
W
N
DROP ALL BUT TORCH
GET TRUNK
N
N
GET TRIDENT
S
S
S
SW
SW
W
S
U
PUT TRUNK AND TRIDENT IN CASE
D
N
E
N
NE
N
GET ALL
N
N
U
N
N
W
N
W
DROP TORCH
TURN ON LAMP
N
GET FIGURINE
S
DROP FIGURINE
N
E
DROP ALL BUT LAMP AND GARLIC
N
D
E
NE
SE
SW
D
D
S
GET COAL
N
U
U
N
E
S
N
U
S
GET ALL
PUT COAL AND SCREWDRIVER IN BASKET
LIGHT MATCH
LIGHT CANDLES WITH MATCH
PUT CANDLES IN BASKET
LOWER BASKET
DROP MATCHBOOK
N
D
E
NE
SE
SW
D
D
W
DROP ALL
W
GET COAL AND CANDLES AND SCREWDRIVER
S
OPEN LID
PUT COAL IN MACHINE
CLOSE LID
TURN SWITCH WITH SCREWDRIVER
DROP SCREWDRIVER
OPEN LID
GET DIAMOND
N
PUT DIAMOND IN BASKET
DROP CANDLES
E
GET ALL BUT TIMBER
E
U
U
N
E
S
N
GET SAPPHIRE
U
S
RAISE BASKET
GET DIAMOND
W
S
GET ALL
E
S
D
U
DROP GARLIC
PUT ALL BUT LAMP IN CASE
D
N
E
N
NE
N
N
GET PUMP
S
S
E
GET PLASTIC
S
D
E
E
S
DROP PLASTIC
INFLATE PLASTIC
DROP PUMP
ENTER BOAT
LAUNCH
GET BUOY
LAND
GET OUT OF BOAT
S
W
W
SW
U
U
NW
W
W
W
OPEN BUOY
PUT EMERALD IN CASE
E
E
N
W
SW
W
]]>

        Await Test(Zork1, script, expected)
    End Function

    Private Function Test(gameName As String, expected As XCData) As Task
        Return Test(gameName, Nothing, expected)
    End Function

    Private Async Function Test(gameName As String, script As XCData, expected As XCData) As Task
        Dim memory = GameMemory(gameName)
        Dim machine = New Machine(memory, debugging:=False)

        machine.Randomize(42)

        Dim screen = New MockScreen(script)
        machine.RegisterScreen(screen)

        Try
            Await machine.RunAsync()
        Catch ex As Exceptions.ZMachineQuitException
        End Try

        Dim expectedText = expected.Value.Trim()

        Assert.Equal(expectedText, screen.Output.Trim())
    End Function

End Class

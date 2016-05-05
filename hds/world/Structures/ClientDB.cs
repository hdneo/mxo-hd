using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace hds.world.Structures
{
    public class ClientDB
    {


        // EXP TABLE
        public static readonly int[] EXP = new int[] 
        {
            0, 500, 5000, 21000, 60000, 137500, 273000,
                       490000, 816000, 1282500, 1925000,2722500, 
                       3690000, 4842500, 6195000, 7762500, 9560000,
                       11602500, 13905000, 16482500, 19350000, 22522500,
                       26015000, 29842500, 34020000, 38562500, 43485000,
                       48802500, 54530000, 60682500, 67275000, 74322500, 
                       81840000, 89842500, 98345000, 107362500, 116910000,
                       127002500, 137655000, 148882500, 160700000, 173122500,
                       186165000, 199842500, 214170000, 229162500, 244835000,
                       261202500, 278280000, 296082500, 314625000
        };

        

        public static Dictionary<uint,byte> EmoteListDict = new Dictionary<uint,byte>() {
            {0xe6020058, 0x1 }, // /beckon
            {0x9b010058, 0x2 }, // /bigwave
            {0xe7020058, 0x3 }, // /bow
            {0x0d00003a, 0x4 }, // /clap
            {0x1000003a, 0x5 }, // /crossarms
            {0x1100003a, 0x6 }, // /nod
            {0x570d0058, 0x7 }, // /agree
            {0x590d0058, 0x8 }, // /yes
            {0x1600003a, 0x9 }, // /orangutan
            {0x10050004, 0xa }, // /point
            {0xd1020058, 0xb }, // /pointback
            {0x1300003a, 0xc }, // /pointleft
            {0x1400003a, 0xd }, // /pointright
           
        };
            
        



        // Animations 
        public enum AnimationList : uint
        {
            // From http://mxoemu.info/mxoanim/action.txt
            IDLE = 0x00,
            WALKF = 0x01,
            WALKB = 0x02,
            RUN = 0x03,
            TURNR = 0x04,
            TURNL = 0x05,
            PUSHF = 0x06,
            JUMPF = 0x07,
            JUMPCAR = 0x08,
            JUMPCARLEN = 0x09,
            LOOKAROUND = 0x0a,
            READBOARD = 0x0b,
            READ = 0x0c,
            TYPE = 0x0d,
            ONTOP = 0x0e,
            OFFTOP = 0x0f,
            ONBOTTOM = 0x10,
            OFFBOTTOM = 0x11,
            CLIMBUP = 0x12,
            CLIMBDOWN = 0x13,
            FIGHTMODE = 0x14,
            CLIPMODE = 0x15,
            BUMPBACKLEFT = 0x16,
            BUMPBACKRIGHT = 0x17,
            BUMPFRONTLEFT = 0x18,
            BUMPFRONTRIGHT = 0x19,
            HYPERSPEED = 0x1a,
            SIZZLE1 = 0x1b,
            SIZZLE2 = 0x1c,
            SIZZLE3 = 0x1d,
            SIZZLE4 = 0x1e,
            BLINDED = 0x1f,
            BULLETPROOF = 0x20,
            DISGUISE = 0x21,
            INVISIBLITY = 0x22,
            IMPAIREDCONNECTION = 0x23,
            ACTIVATEDEVICE = 0x24,
            EMOTE_ = 0x25,
            OVERCLOCK = 0x26,
            GAUSSIANBLUR = 0x27,
            SHADOWON = 0x28,
            EXTENDED_ = 0x29,
            AGENTTRANSFORM = 0x2a,
            AGENTTRANSFORMFADE = 0x2b,
            SEEPLAYER = 0x2c,
            HEARSOUND = 0x2d,
            HEARSOUNDRIGHT = 0x2e,
            HEARSOUNDLEFT = 0x2f,
            HEARSOUND180 = 0x30,
            ABILPROGRAMLAUNCHA = 0x31,
            ABILPROGRAMLAUNCHA1 = 0x32,
            COMBATATTACKERRANDOMSCRAMBLE = 0x33,
            FREEATTACKMELEESOLDIR = 0x34,
            FREEATTACKMELEESPY = 0x35,
            FREEATTACKMELEEHACKER = 0x36,
            FREEATTACKRIFLE = 0x37,
            FREEATTACKPISTOL = 0x38,
            FREEATTACKWEAPONSUBM = 0x39,
            FREEATTACKWEAPONDUALPISTOL = 0x3a,
            FREEATTACKWEAPONDUALSUBM = 0x3b,
            STRAFERUNL = 0x3c,
            STRAFERUNR = 0x3d

            /*
             * From http://mxoemu.info/mxoanim/action.txt
             * ToDo: Build a real AnimationList :)
                
                
                3e "RunFEnd" 
                3f "Resurrect" 
                40 "FreeAttackWeaponSMKnifeThrow" 
                41 "Abil_RecallMissionTeam" 
                42 "SweepkickAll" 
                43 "SynapticKiBurstAll" 
                44 "Abil_SelfHeal_A" 
                45 "FailedOpportunityAttackSR" 
                46 "FailedOpportunityAttackMRF250" 
                47 "ReloadRifle" 
                48 "ReloadPistol" 
                49 "ReloadSubmachinegun" 
                4a "ReloadDualPistol" 
                4b "ReloadDualSubmachinegun" 
                4c "CarBump_BackLeft" 
                4d "CarBump_BackRight" 
                4e "CarBump_FrontLeft" 
                4f "CarBump_FrontRight" 
                50 "DTapRunF" 
                51 "DTapWalkB" 
                52 "DTapRight" 
                53 "DTapLeft" 
                54 "FreeAttackWeaponSMPistolEvasion" 
                55 "FreeAttackWeaponSMPistolBarrage" 
                56 "FreeAttackWeaponSMPistolDisarmingShot" 
                57 "FreeAttackWeaponSMRifleBodyShot" 
                58 "FreeAttackWeaponSMRifleImmobilizingShot" 
                59 "FreeAttackWeaponSMRifleRifleThreeRoundB" 
                5a "FreeAttackWeaponSMSubMBulletSpray" 
                5b "FreeAttackWeaponSMSubMSuppressionFire" 
                5c "FreeAttackWeaponSMSubMControlledBurst" 
                5d "FreeAttackWeaponSMPistolHinderingShot" 
                5e "FreeAttackWeaponSMRifleSteadyAim" 
                5f "FreeAttackWeaponSMRifleCripplingShot" 
                60 "FreeAttackWeaponSMRifleDeadlyShot" 
                61 "ShotFromLF" 
                62 "ShotFromLL" 
                63 "ShotFromLR" 
                64 "ShotFromLB" 
                65 "ShotFromMF" 
                66 "ShotFromML" 
                67 "ShotFromMR" 
                68 "ShotFromMB" 
                69 "ShotFromHF" 
                6a "ShotFromHL" 
                6b "ShotFromHR" 
                6c "ShotFromHB" 
                6d "Emote_Eyedrops" 
                6e "Emote_Drinkbottle" 
                6f "Emote_Slappatch" 
                70 "emote_Takepill" 
                71 "Emote_TakeInhaler" 
                72 "Emote_Useairhypo" 
                73 "ChatNormal" 
                74 "ChatFaction" 
                75 "ChatShout" 
                76 "ChatFaction" 
                77 "ChatFaction" 
                78 "ChatFaction" 
                79 "FreeAttackWeaponSMDualSubMDirectFire" 
                7a "FreeAttackWeaponSMDualSubMFullAuto" 
                7b "FreeAttackWeaponSMDualSubMFullAutoRedux" 
                7c "FreeAttackWeaponSMDualSubMPinningFire" 
                7d "FreeAttackWeaponSMMiniFlashBomb" 
                7e "FreeAttackWeaponSMSubMCoveringFire" 
                7f "Emote_Karatespeed1" 
                80 "Emote_Kungfupower1" 
                81 "Emote_Aikidospeed1" 
                82 "RunB" 
                83 "Abil_DeflectVirus_D" 
                84 "Abil_ProgLauCoderGeneric2s_A" 
                85 "Abil_ProgLauCoderGeneric4s_A" 
                86 "Abil_ProgLauCoderGeneric6s_A" 
                87 "Abil_ProgLauCoderGeneric8s_A" 
                88 "Abil_ProgLauCoderSimulacra2s_A" 
                89 "Abil_ProgLauCoderSimulacra4s_A" 
                8a "Abil_ProgLauCoderSimulacra6s_A" 
                8b "Abil_ProgLauCoderSimulacra8s_A" 
                8c "Abil_ProgLauGuardianPatcher2s_A" 
                8d "Abil_ProgLauGuardianPatcher4s_A" 
                8e "Abil_ProgLauGuardianPatcher6s_A" 
                8f "Abil_ProgLauGuardianPatcher8s_A" 
                90 "Abil_ProgLauNetworkHacker2s_A" 
                91 "Abil_ProgLauNetworkHacker4s_A" 
                92 "Abil_ProgLauNetworkHacker6s_A" 
                93 "Abil_ProgLauNetworkHacker8s_A" 
                94 "Abil_ProgLauPathologist2s_A" 
                95 "Abil_ProgLauPathologist4s_A" 
                96 "Abil_ProgLauPathologist6s_A" 
                97 "Abil_ProgLauPathologist8s_A" 
                98 "Abil_ProgLauSelectivePhage2s_A" 
                99 "Abil_ProgLauSelectivePhage4s_A" 
                9a "Abil_ProgLauSelectivePhage6s_A" 
                9b "Abil_ProgLauSelectivePhage8s_A" 
                9c "Abil_ProgLauTeamPatcher2s_A" 
                9d "Abil_ProgLauTeamPatcher4s_A" 
                9e "Abil_ProgLauTeamPatcher6s_A" 
                9f "Abil_ProgLauTeamPatcher8s_A" 
                a0 "Abil_ProgramLaunch2s_A" 
                a1 "Abil_ProgramLaunch4s_A" 
                a2 "Abil_ProgramLaunch6s_A" 
                a3 "Abil_ProgramLaunch8s_A" 
                a4 "Emote_TakePill" 
                a5 "DisguiseOff" 
                a6 "StrafeRunL" 
                a7 "StrafeRunL" 
                a8 "StrafeRunL" 
                a9 "StrafeRunL" 
                aa "StrafeRunR" 
                ab "StrafeRunR" 
                ac "StrafeRunR" 
                ad "StrafeRunR" 
                ae "Abil_ProgLauEngageFoes2s_A" 
                af "Abil_DataTap6s_A" 
                b0 "Abil_DataTap8s_A" 
                b1 "Abil_DataNodeLocate2s_A" 
                b2 "Concealment" 
                b3 "Abil_PerfectSelf_A" 
                b4 "IntenseAttack" 
                b5 "Abil_Spray_A" 
                b6 "Abil_CorruptingBile_A" 
                b7 "Abil_WaveofCorruption_A" 
                b8 "FreeAttackKarate1" 
                b9 "FreeAttackKarate2" 
                ba "FreeAttackKarate3" 
                bb "FreeAttackWushu1" 
                bc "FreeAttackWushu2" 
                bd "FreeAttackWushu3" 
                be "FreeAttackAikido1" 
                bf "FreeAttackAikido2" 
                c0 "FreeAttackAikido3" 
                c1 "FreeAttackSelfD1" 
                c2 "FreeAttackSelfD2" 
                c3 "FreeAttackSelfD3" 
                c4 "HitFromLF" 
                c5 "HitFromLL" 
                c6 "HitFromLR" 
                c7 "HitFromLB" 
                c8 "HitFromMF" 
                c9 "HitFromML" 
                ca "HitFromMR" 
                cb "HitFromMB" 
                cc "HitFromHF" 
                cd "HitFromHL" 
                ce "HitFromHR" 
                cf "HitFromHB" 
                d0 "FreeAttackWeaponFlitGun" 
                d1 "FreeAttackWeaponSMPistolExecution" 
                d2 "FreeAttackWeaponSM3RoundBurst" 
                d3 "Emote_Cry" 
                d4 "Emote_Extracheer" */

        }


        /*
             * FULL LIST
             * ToDo: Build a real AnimationList :)
             *  00 "Idle" 
                01 "WalkF" 
                02 "WalkB" 
                03 "RunF" 
                04 "TurnR" 
                05 "TurnL" 
                06 "PushF" 
                07 "JumpF" 
                08 "JumpCar" 
                09 "JumpCarLength" 
                0a "LookAround" 
                0b "ReadBoard" 
                0c "Read" 
                0d "Type" 
                0e "OnTop" 
                0f "OffTop" 
                10 "OnBottom" 
                11 "OffBottom" 
                12 "Climb_Up" 
                13 "Climb_Down" 
                14 "FightMode" 
                15 "ClipMode" 
                16 "Bump_BackLeft" 
                17 "Bump_BackRight" 
                18 "Bump_FrontLeft" 
                19 "Bump_FrontRight" 
                1a "HyperSpeed" 
                1b "Sizzle1" 
                1c "Sizzle2" 
                1d "Sizzle3" 
                1e "Sizzle4" 
                1f "Blinded" 
                20 "Bulletproof" 
                21 "Disguise" 
                22 "Invisibility" 
                23 "Impaired_connection" 
                24 "ActivateDevice" 
                25 "emote_" 
                26 "Overclock" 
                27 "GaussianBlur" 
                28 "ShadowOn" 
                29 "extended_" 
                2a "AgentTransform" 
                2b "AgentTransform_Fade" 
                2c "SeePlayer" 
                2d "HearSound" 
                2e "HearSoundRight" 
                2f "HearSoundLeft" 
                30 "HearSound180" 
                31 "Abil_ProgramLaunch_A" 
                32 "Abil_ProgramLaunch_A" 
                33 "CombatAttackerRandomScramble" 
                34 "FreeAttackMeleeSoldier" 
                35 "FreeAttackMeleeSpy" 
                36 "FreeAttackMeleeHacker" 
                37 "FreeAttackWeaponRifle" 
                38 "FreeAttackWeaponPistol" 
                39 "FreeAttackWeaponSubM" 
                3a "FreeAttackWeaponDualPistol" 
                3b "FreeAttackWeaponDualSubM" 
                3c "StrafeRunL" 
                3d "StrafeRunR" 
                3e "RunFEnd" 
                3f "Resurrect" 
                40 "FreeAttackWeaponSMKnifeThrow" 
                41 "Abil_RecallMissionTeam" 
                42 "SweepkickAll" 
                43 "SynapticKiBurstAll" 
                44 "Abil_SelfHeal_A" 
                45 "FailedOpportunityAttackSR" 
                46 "FailedOpportunityAttackMRF250" 
                47 "ReloadRifle" 
                48 "ReloadPistol" 
                49 "ReloadSubmachinegun" 
                4a "ReloadDualPistol" 
                4b "ReloadDualSubmachinegun" 
                4c "CarBump_BackLeft" 
                4d "CarBump_BackRight" 
                4e "CarBump_FrontLeft" 
                4f "CarBump_FrontRight" 
                50 "DTapRunF" 
                51 "DTapWalkB" 
                52 "DTapRight" 
                53 "DTapLeft" 
                54 "FreeAttackWeaponSMPistolEvasion" 
                55 "FreeAttackWeaponSMPistolBarrage" 
                56 "FreeAttackWeaponSMPistolDisarmingShot" 
                57 "FreeAttackWeaponSMRifleBodyShot" 
                58 "FreeAttackWeaponSMRifleImmobilizingShot" 
                59 "FreeAttackWeaponSMRifleRifleThreeRoundB" 
                5a "FreeAttackWeaponSMSubMBulletSpray" 
                5b "FreeAttackWeaponSMSubMSuppressionFire" 
                5c "FreeAttackWeaponSMSubMControlledBurst" 
                5d "FreeAttackWeaponSMPistolHinderingShot" 
                5e "FreeAttackWeaponSMRifleSteadyAim" 
                5f "FreeAttackWeaponSMRifleCripplingShot" 
                60 "FreeAttackWeaponSMRifleDeadlyShot" 
                61 "ShotFromLF" 
                62 "ShotFromLL" 
                63 "ShotFromLR" 
                64 "ShotFromLB" 
                65 "ShotFromMF" 
                66 "ShotFromML" 
                67 "ShotFromMR" 
                68 "ShotFromMB" 
                69 "ShotFromHF" 
                6a "ShotFromHL" 
                6b "ShotFromHR" 
                6c "ShotFromHB" 
                6d "Emote_Eyedrops" 
                6e "Emote_Drinkbottle" 
                6f "Emote_Slappatch" 
                70 "emote_Takepill" 
                71 "Emote_TakeInhaler" 
                72 "Emote_Useairhypo" 
                73 "ChatNormal" 
                74 "ChatFaction" 
                75 "ChatShout" 
                76 "ChatFaction" 
                77 "ChatFaction" 
                78 "ChatFaction" 
                79 "FreeAttackWeaponSMDualSubMDirectFire" 
                7a "FreeAttackWeaponSMDualSubMFullAuto" 
                7b "FreeAttackWeaponSMDualSubMFullAutoRedux" 
                7c "FreeAttackWeaponSMDualSubMPinningFire" 
                7d "FreeAttackWeaponSMMiniFlashBomb" 
                7e "FreeAttackWeaponSMSubMCoveringFire" 
                7f "Emote_Karatespeed1" 
                80 "Emote_Kungfupower1" 
                81 "Emote_Aikidospeed1" 
                82 "RunB" 
                83 "Abil_DeflectVirus_D" 
                84 "Abil_ProgLauCoderGeneric2s_A" 
                85 "Abil_ProgLauCoderGeneric4s_A" 
                86 "Abil_ProgLauCoderGeneric6s_A" 
                87 "Abil_ProgLauCoderGeneric8s_A" 
                88 "Abil_ProgLauCoderSimulacra2s_A" 
                89 "Abil_ProgLauCoderSimulacra4s_A" 
                8a "Abil_ProgLauCoderSimulacra6s_A" 
                8b "Abil_ProgLauCoderSimulacra8s_A" 
                8c "Abil_ProgLauGuardianPatcher2s_A" 
                8d "Abil_ProgLauGuardianPatcher4s_A" 
                8e "Abil_ProgLauGuardianPatcher6s_A" 
                8f "Abil_ProgLauGuardianPatcher8s_A" 
                90 "Abil_ProgLauNetworkHacker2s_A" 
                91 "Abil_ProgLauNetworkHacker4s_A" 
                92 "Abil_ProgLauNetworkHacker6s_A" 
                93 "Abil_ProgLauNetworkHacker8s_A" 
                94 "Abil_ProgLauPathologist2s_A" 
                95 "Abil_ProgLauPathologist4s_A" 
                96 "Abil_ProgLauPathologist6s_A" 
                97 "Abil_ProgLauPathologist8s_A" 
                98 "Abil_ProgLauSelectivePhage2s_A" 
                99 "Abil_ProgLauSelectivePhage4s_A" 
                9a "Abil_ProgLauSelectivePhage6s_A" 
                9b "Abil_ProgLauSelectivePhage8s_A" 
                9c "Abil_ProgLauTeamPatcher2s_A" 
                9d "Abil_ProgLauTeamPatcher4s_A" 
                9e "Abil_ProgLauTeamPatcher6s_A" 
                9f "Abil_ProgLauTeamPatcher8s_A" 
                a0 "Abil_ProgramLaunch2s_A" 
                a1 "Abil_ProgramLaunch4s_A" 
                a2 "Abil_ProgramLaunch6s_A" 
                a3 "Abil_ProgramLaunch8s_A" 
                a4 "Emote_TakePill" 
                a5 "DisguiseOff" 
                a6 "StrafeRunL" 
                a7 "StrafeRunL" 
                a8 "StrafeRunL" 
                a9 "StrafeRunL" 
                aa "StrafeRunR" 
                ab "StrafeRunR" 
                ac "StrafeRunR" 
                ad "StrafeRunR" 
                ae "Abil_ProgLauEngageFoes2s_A" 
                af "Abil_DataTap6s_A" 
                b0 "Abil_DataTap8s_A" 
                b1 "Abil_DataNodeLocate2s_A" 
                b2 "Concealment" 
                b3 "Abil_PerfectSelf_A" 
                b4 "IntenseAttack" 
                b5 "Abil_Spray_A" 
                b6 "Abil_CorruptingBile_A" 
                b7 "Abil_WaveofCorruption_A" 
                b8 "FreeAttackKarate1" 
                b9 "FreeAttackKarate2" 
                ba "FreeAttackKarate3" 
                bb "FreeAttackWushu1" 
                bc "FreeAttackWushu2" 
                bd "FreeAttackWushu3" 
                be "FreeAttackAikido1" 
                bf "FreeAttackAikido2" 
                c0 "FreeAttackAikido3" 
                c1 "FreeAttackSelfD1" 
                c2 "FreeAttackSelfD2" 
                c3 "FreeAttackSelfD3" 
                c4 "HitFromLF" 
                c5 "HitFromLL" 
                c6 "HitFromLR" 
                c7 "HitFromLB" 
                c8 "HitFromMF" 
                c9 "HitFromML" 
                ca "HitFromMR" 
                cb "HitFromMB" 
                cc "HitFromHF" 
                cd "HitFromHL" 
                ce "HitFromHR" 
                cf "HitFromHB" 
                d0 "FreeAttackWeaponFlitGun" 
                d1 "FreeAttackWeaponSMPistolExecution" 
                d2 "FreeAttackWeaponSM3RoundBurst" 
                d3 "Emote_Cry" 
                d4 "Emote_Extracheer" */
        


    }
}

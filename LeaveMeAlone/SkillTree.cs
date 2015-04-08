﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using LeaveMeAlone;
using Microsoft.Xna.Framework;

namespace LeaveMeAlone
{
    public class SkillTree
    {
        public Dictionary<int, List<Skill>> skill_tiers;
        public Dictionary<int, List<Room>> room_tiers;

        public static Texture2D spike_room_image;
        public static Texture2D poison_pit_image;
        
        public static Dictionary<Character.Type, SkillTree> skilltrees = new Dictionary<Character.Type, SkillTree>();
        public Dictionary<Skill, Button> SkillButtons = new Dictionary<Skill,Button>();
        public ButtonRoom[] AvailableRooms = new ButtonRoom[2];
        public static Texture2D buttonPic;

        Vector2 baseSkillButtonPos = new Vector2(300, 50);
        Vector2 baseRoomButtonPos = new Vector2(700, 50);
        Vector2 baseSelectedSkillButtonPos = new Vector2(20, 100);

        //>>>>>>>>>>>>>>>>>>>>Skill Declarations<<<<<<<<<<<<//
        public static Skill basic_attack;
        public static Skill defend;
        public static Skill portal_punch;
        public static Skill flamethrower;
        public static Skill nuclear_waste;
        public static Skill abomination_form;
        public static Skill summon_igor;
        public static Skill freeze_ray;

        public static Skill cure;
        public static Skill panacea;
        public static Skill fire;
        public static Skill bash;
        public static Skill poison_dagger;
        public static Skill haste;

        //>>>>>>>>>>>>>>>>Room Declarations<<<<<<<<<<<//
        public static Room spike_trap;
        public static Room poison_pit;

        public struct ButtonRoom
        {
            public Button b;
            private Room r;
            private bool drawable;
            public void Draw(SpriteBatch s)
            {
                if(drawable)
                {
                    b.Draw(s);
                }
            }
            public void UpdateRoom(Room r)
            {
                b.text.changeMessage(r.name);
                drawable = true;
            }
            public void DeleteRoom()
            {
                r = default(Room);
                drawable = false;
            }
        }
        public SkillTree()
        {
            skill_tiers = new Dictionary<int, List<Skill>>();
            room_tiers = new Dictionary<int, List<Room>>();
            AvailableRooms[0] = new ButtonRoom();
            AvailableRooms[1] = new ButtonRoom();
            AvailableRooms[0].b = new Button(buttonPic, (int)baseRoomButtonPos.X, (int)baseRoomButtonPos.Y, 100, 100);
            AvailableRooms[1].b = new Button(buttonPic, (int)baseRoomButtonPos.X + 75, (int)baseRoomButtonPos.Y, 100, 100);
        }
        public static void LoadContent(ContentManager content)
        {
            spike_room_image = content.Load<Texture2D>("spikeRoom2");
            poison_pit_image = content.Load<Texture2D>("PoisonPit");
            buttonPic = content.Load<Texture2D>("buttonbase");

            //>>>>>>>>>>>>>>>>>>>>Skill Instances<<<<<<<<<<<<<<<<<<<//
            basic_attack = new Skill("Attack", 0, 0, 1, 0, Skill.Target.Single, 0, "Basic Attack", BasicAttack);
            defend = new Skill("Defend", 0, 0, 1, 1, Skill.Target.Self, 0, "Heal yourself!", Defend);
            
            //>>>>>>>>>>>>>>>>>>>>Boss Skill Instances<<<<<<<<<<<<<<<<<<<<//
            portal_punch = new Skill("Portal Punch", 5, 0, 1, 0, Skill.Target.Single, 1, "Does Sp.Atk. Dmg", PortalPunch);
            flamethrower = new Skill("Flamethrower", 10, 0, 1, 0, Skill.Target.All, 1, "Burn all of your enemies!", FlameThrower);
            nuclear_waste = new Skill("Nuclear Waste", 5, 0, 1, 0, Skill.Target.Single, 1, "Infect an enemy with poision", NuclearWaste);
            abomination_form = new Skill("Abomination Form", 10, 10, 5, 3, Skill.Target.All, 1, "Science Gone Astray! Swap Atk and Sp. Atk", AbominationForm);
            summon_igor = new Skill("Summon Igor", 5, 300, 2, 1, Skill.Target.Single, 1, "Summon your minion to prod away the heroes", SummonIgor);
            freeze_ray = new Skill("FreezeRay", 15, 2500, 20, 2, Skill.Target.All, 1, "Freeze all enemies", FreezeRay);
            
            //>>>>>>>>>>>>>>>>>>>>>Hero Skill Instances<<<<<<<<<<<<<<<<<<<//

            cure = new Skill("cure", 5, 0 ,1, 1, Skill.Target.Single, 1, "Heals and ally or self", Cure);
            fire = new Skill("fire", 5, 0, 1, 1, Skill.Target.Single, 1, "Burn an enemy", Fire);
            bash = new Skill("bash", 5, 0 ,1, 1, Skill.Target.Single, 1, "Hit an enemy using physical attack", Bash);
            haste = new Skill("haste", 15, 0, 5, 3, Skill.Target.Single, 1, "Speed an ally up so he can hit twice in a row", Haste);
            panacea = new Skill("panacea", 10, 0, 3, 0, Skill.Target.Single, 1, "Cure Self or Ally of all Status effects", Panacea);
            poison_dagger = new Skill("poison_dagger", 5, 0, 1, 1, Skill.Target.Single, 1, "Do physical damage and give poison", PoisonDagger);
            //>>>>>>>>>>>>>>>>>>>Room Instances<<<<<<<<<<<<<<<<<<<<<//
            spike_trap = new Room("Spike Trap", 100, 1, 0, "Does damage to hero relative to their defense", SpikeTrap, spike_room_image);
            poison_pit = new Room("Poison Pit", 100, 1, 0, "Has 50% chance of infecting each hero with poison", PoisonPit, poison_pit_image);
        }

        //Updates or creates the buttons and 
        public void updateTree()
        {
            List<int> keys = skill_tiers.Keys.ToList();
            keys.Sort();
            //int boss_level = BattleManager.boss.level;
            //used to separate buttons horizontally by level
            int kindex = 0;
            foreach (int key in keys)
            {
                Console.WriteLine(key);
                List<Skill> skilltier = skill_tiers[key];
                int slength = skilltier.Count;
                int sindex = 0;
                foreach (Skill skill in skilltier)
                {
                    Console.WriteLine(skill.name);
                    Button b = new Button(buttonPic, (int)baseSkillButtonPos.X + sindex*175, (int)baseSkillButtonPos.Y + 75*kindex, 150, 50);
                    b.UpdateText(skill.name);
                    SkillButtons[skill] = b;
                    sindex++;
                }
                kindex++;
            }
        }


        //Instantiates all classes
        public static void Init()
        {
            initBrute();
            initMastermind();
            initOperative();
            initRanger();
            initMage();
            initKnight();

        }
        public void Draw(SpriteBatch s)
        {
            foreach(Button button in SkillButtons.Values)
            {
                button.Draw(s);
            }


        }
        public void addSkill(int level, Skill skill)
        {
            addToDict(skill_tiers, ref level, ref skill);
        }
        public void addRoom(int level, Room room)
        {
            addToDict(room_tiers, ref level, ref room);
        }
        private void addToDict<T,F>(Dictionary<T,List<F>> d, ref T index, ref F value)
        {
            List<F> existing;
            if (!d.TryGetValue(index, out existing))
            {
                existing = new List<F>();
                d[index] = existing;
            }
            // At this point we know that "existing" refers to the relevant list in the 
            // dictionary, one way or another.
            existing.Add(value);
        }


        //The thing with all the trees
        
        public static void initBrute()
        {
            SkillTree st = new SkillTree();
            st.addSkill(1, portal_punch);
            st.addSkill(1, flamethrower);
            st.addSkill(2, nuclear_waste);

            st.addRoom(1, spike_trap);
            st.addRoom(1, poison_pit);
            skilltrees[Character.Type.Brute] = st;
            st.updateTree();

        }
        public static void initMastermind()
        {
            SkillTree st = new SkillTree();
            st.addSkill(1, portal_punch);
            st.addSkill(1, flamethrower);
            st.addSkill(2, nuclear_waste);
            st.addSkill(2, abomination_form);
            st.addSkill(3, summon_igor);
            st.addSkill(3, freeze_ray);

            st.addRoom(1, spike_trap);
            st.addRoom(1, poison_pit);
            skilltrees[Character.Type.Mastermind] = st;
            st.updateTree();

        }
        public static void initOperative()
        {
            SkillTree st = new SkillTree();
            st.addSkill(1, portal_punch);
            st.addSkill(1, flamethrower);
            st.addSkill(2, nuclear_waste);

            st.addRoom(1, spike_trap);
            st.addRoom(1, poison_pit);
            skilltrees[Character.Type.Operative] = st;
            st.updateTree();

        }
        public static void initRanger()
        {
            SkillTree st = new SkillTree();
            //addSkill(level, skill)
            skilltrees[Character.Type.Ranger] = st;
        }
        public static void initMage()
        {
            SkillTree st = new SkillTree();
            st.addSkill(1, portal_punch);
            st.addSkill(1, flamethrower);
            st.addSkill(2, nuclear_waste);
            skilltrees[Character.Type.Mage] = st;
            st.updateTree();

        }
        public static void initKnight()
        {
            SkillTree st = new SkillTree();
            //addSkill(level, skill)
            skilltrees[Character.Type.Knight] = st;
            st.updateTree();
        }

        

        //>>>>>>>>>>>>>>>>>>>>Skill Delegates<<<<<<<<<<<<<<<<<<<//
        public static void BasicAttack(Character caster, Character target = null)
        {
            int damage = Skill.damage(caster, target, Skill.Attack.Attack, Skill.Defense.Defense, 100);
            target.health -= damage;
            String str_damage = (-damage).ToString();
            target.damage_text.changeMessage(str_damage);
        }
        public static void Defend(Character caster, Character target = null)
        {
            caster.health += (int)(((double)caster.max_health) * .2);
            caster.energy += 10;
            if (caster.health > caster.max_health)
            {
                caster.health = caster.max_health;
            }
            if (caster.energy > caster.max_energy)
            {
                caster.energy = caster.max_energy;
            }
            
            //If the status already exists, increase its duration
            //Status this_defend = new Status("defend", 2, Status.Effect_Time.Once, Status.Type.Buff, Status.defplus_image, Status.DoNothing, Status.ReduceDefense);
            //We use DoNothing here^ because we raise defense here, and it is lowered in 2 turns

            if (caster.statuses.Contains(Status.check_defend))
            {
                int status_index = caster.statuses.IndexOf(Status.check_defend);
                caster.statuses[status_index].duration_left += 2;
            }
            //Otherwise add it
            else
            {
                caster.statuses.Add(new Status("defend", 2, 0, Status.Effect_Time.Once, Status.Type.Buff, Status.defplus_image, Status.DoNothing, Status.ReduceDefense));
                caster.defense += (5 * 1 + (caster.level / 3));
            }
            //Status this_sdefend = new Status("specdefend", 2, Status.Effect_Time.Once, Status.Type.Buff, Status.defplus_image, Status.DoNothing, Status.ReduceSDefense);
            //If the status already exists, increase its duration
            if (caster.statuses.Contains(Status.check_specdefend))
            {
                int status_index = caster.statuses.IndexOf(Status.check_specdefend);
                caster.statuses[status_index].duration_left += 2;
            }
            //Otherwise add it
            else
            {
                caster.statuses.Add(new Status("specdefend", 2, 0, Status.Effect_Time.Once, Status.Type.Buff, Status.defplus_image, Status.DoNothing, Status.ReduceSDefense)); 
                caster.special_defense += (5 * 1 + (caster.level / 3));
            }
        }
        public static void PortalPunch(Character caster, Character target = null)
        {
            int damage = Skill.damage(caster, target, Skill.Attack.SpecialAttack, Skill.Defense.SpecialDefense, 100);
            target.health -= damage;
            String str_damage = (-damage).ToString();
            target.damage_text.changeMessage(str_damage);
        }
        public static void FlameThrower(Character caster, Character target = null)
        {
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    target = BattleManager.heroes[i];
                    if (target == null) { continue; }
                    int damage = Skill.damage(caster, target, Skill.Attack.SpecialAttack, Skill.Defense.SpecialDefense, 40);
                    target.health -= damage;
                    String str_damage = (-damage).ToString();
                    target.damage_text.changeMessage(str_damage);
                }
                catch (ArgumentOutOfRangeException)
                {

                }
            }          
        }
        public static void NuclearWaste(Character caster, Character target = null)
        {
            int damage = Skill.damage(caster, target, Skill.Attack.SpecialAttack, Skill.Defense.SpecialDefense, 40);
            target.health -= damage;
            target.damage_text.changeMessage((-damage).ToString());

            //Status this_poison = new Status("poison", 3, Status.Effect_Time.After, Status.Type.Debuff, Status.poison_image, Status.Poison);
            //If the status already exists, increase its duration
            if (target.statuses.Contains(Status.check_poison))
            {
                int status_index = target.statuses.IndexOf(Status.check_poison);
                target.statuses[status_index].duration_left += 3;
            }
            //Otherwise add it
            else
            {
                target.statuses.Add(new Status("poison", 3, 0, Status.Effect_Time.After, Status.Type.Debuff, Status.poison_image, Status.Poison));
            }
        }
        public static void AbominationForm(Character caster, Character target = null)
        {
            //Change Sprite! or Something!
            int temp = caster.attack;
            caster.attack = caster.special_attack;
            caster.special_attack = temp;

            if (caster.statuses.Contains(Status.check_abom))
            {
                caster.statuses.Remove(Status.check_abom);
            }
            //Otherwise add it
            else
            {
                caster.statuses.Add(new Status("abom", 999, 0, Status.Effect_Time.Once, Status.Type.Other, null, Status.DoNothing, Status.rev_Abom));
            }
        }
        public static void SummonIgor(Character caster, Character target = null)
        {
            //No need to check if it's already there based on the nature of the skill
            //There is a hack here where I use an already instantiated Status to get the delegate function for Summoning Igor
            Status igor_target = new Status("Igor", 2, caster.special_attack, Status.Effect_Time.Once, Status.Type.Other, Status.target_status_image, Status.DoNothing, null);
            igor_target.reverse_affect = igor_target.rev_Igor;
            target.statuses.Add(igor_target);
        }
        public static void FreezeRay(Character caster, Character target = null)
        {
            foreach (Character a_target in BattleManager.heroes)
            {
                a_target.statuses.Add(new Status("stun", LeaveMeAlone.random.Next(1,4), 0, Status.Effect_Time.Once, Status.Type.Debuff, Status.stun_image, Status.DoNothing));
            }
        }

        //>>>>>>>>>>>>>>>>>Hero Skill Delegates<<<<<<<<<<<<<<<<<//
        public static void Cure(Character caster, Character target = null)
        {
            int heal_pts = caster.special_attack;
            target.health += heal_pts;
        }
        public static void Fire(Character caster, Character target = null)
        {
            {
                int damage = Skill.damage(caster, target, Skill.Attack.SpecialAttack, Skill.Defense.SpecialDefense, 100);
                target.health -= damage;
                String str_damage = (-damage).ToString();
                target.damage_text.changeMessage(str_damage);
            }
        }
        public static void Bash(Character caster, Character target = null)
        {
            {
                int damage = Skill.damage(caster, target, Skill.Attack.Attack, Skill.Defense.Defense, 100);
                target.health -= damage;
                String str_damage = (-damage).ToString();
                target.damage_text.changeMessage(str_damage);
            }
        }
        public static void PoisonDagger(Character caster, Character target = null)
        {
            int damage = Skill.damage(caster, target, Skill.Attack.SpecialAttack, Skill.Defense.SpecialDefense, 40);
            target.health -= damage;
            target.damage_text.changeMessage((-damage).ToString());

            //Status this_poison = new Status("poison", 3, Status.Effect_Time.After, Status.Type.Debuff, Status.poison_image, Status.Poison);
            //If the status already exists, increase its duration
            if (target.statuses.Contains(Status.check_poison))
            {
                int status_index = target.statuses.IndexOf(Status.check_poison);
                target.statuses[status_index].duration_left += 3;
            }
            //Otherwise add it
            else
            {
                target.statuses.Add(new Status("poison", 3, 0, Status.Effect_Time.After, Status.Type.Debuff, Status.poison_image, Status.Poison));
            }
        }
        public static void Haste(Character caster, Character target = null) 
        {
            if (target.statuses.Contains(Status.check_haste))
            {
                //Do Nothing
            }
            //Otherwise add it
            else
            {
                target.statuses.Add(new Status("haste", 3, 0, Status.Effect_Time.Once, Status.Type.Debuff, Status.haste_image, Status.DoNothing));
            }
        }
        public static void Panacea(Character caster, Character target = null)
        {
            for (int i = target.statuses.Count() - 1; i >= 0; i--)
            {
                if (target.statuses[i].type == Status.Type.Debuff)
                {
                    target.statuses.Remove(target.statuses[i]);
                }
            }
        }
        //>>>>>>>>>>>>>>>>>>>>>>>Room Delegates<<<<<<<<<<<<<<<<<<<<//
        public static void SpikeTrap(List<Character> heroes)
        {
            for (int i = 0; i < heroes.Count(); i++)
            {
                Character hero = heroes[i];
                if (hero != null)
                {
                    int damage = (int)(hero.max_health * .1);
                    hero.health -= damage;
                    if (hero.health <= 0)
                    {
                        hero.health = 1;
                    }
                }
            }
        }
        public static void PoisonPit(List<Character> heroes)
        {
            for (int i = 0; i < heroes.Count(); i++)
            {
                Character hero = heroes[i];
                if (hero != null)
                {
                    if (LeaveMeAlone.random.Next(100) < 50)
                    {
                        hero.statuses.Add(new Status("poison", 3, 0, Status.Effect_Time.After, Status.Type.Debuff, Status.poison_image, Status.Poison));
                    }
                }
            }
        }
    }
}

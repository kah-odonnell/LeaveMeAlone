﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
#endregion

namespace LeaveMeAlone
{
    public class BattleManager
    {

        public static Text boss_hp;
        public static Text boss_energy;

        public static List<Character> heroes = new List<Character>(4);
        public static List<Text> hero_hp = new List<Text>(4);
        public static List<Rectangle> heroLoc = new List<Rectangle>();
        public static Point herobase = new Point(75, 120);

        public static Dictionary<Character.Knowledge, bool> Knowledge = new Dictionary<Character.Knowledge,bool>();

        public static Text hovertext = new Text("", new Vector2(LeaveMeAlone.BackgroundRect.X, LeaveMeAlone.BackgroundRect.Y), Text.fonts["RetroComputer-12"]);
        public static Texture2D hovertextbackground;

        public static Character boss;
        public static Rectangle bossLoc;

        private static Button[] basic_buttons = new Button[4];
        private static Button[] skill_buttons = new Button[6];
        private static Texture2D buttonLocPic;
        public static Texture2D bkgd;
        private static Texture2D targeter;
        private static int[] check_cooldown = new int[6];

        private static bool haste_check;

        private static Text info_text;
        private static int info_counter;
        private static Text target_text;

        private static Text victory_text;
        private static bool victory;
        private static bool defeat;
        private static Text defeat_text;

        private static Button next_button;
        private static Button back_button;

        //debug string
        private static Text message; 

        private static bool left_click = false;
        private static bool right_click = false;
        private static int animation_counter = 30;


        private static int enemy_attack_delay = 60;
        private static int enemy_turn = -1;
        private enum State { Basic, Skills, Bribe, Target, Attack, Endgame, EnemyTurn }
        private static State state;


        //TODO figure out if this stuff needs to be numbers or can be Characters
        private static int hovered_enemy = -1;
        private static int targeted_enemy = -1;
        private static Skill selected_skill;

       
        //------------Bribe Stuff------------//
        private static Button[] bribe_amounts = new Button[4];
        private static Button total_amount;
        private static Button my_amount;
        private static int bribe_gold;


        public static void LoadContent(ContentManager Content)
        {


            bkgd = Content.Load<Texture2D>("skyscraperBkgd");

            int button_basex = 300;
            int button_basey = LeaveMeAlone.WindowY - 150;


            //remove all knowledge that the enemy heroes have
            Knowledge.Clear();

            buttonLocPic = Content.Load<Texture2D>("buttonbase");
            targeter = Content.Load<Texture2D>("Target");
            target_text = new Text("Select Target", new Vector2(100, 100), Text.fonts["RetroComputer-12"]);

            basic_buttons[0] = new Button(buttonLocPic, button_basex, button_basey, 250, 50);
            basic_buttons[1] = new Button(buttonLocPic, button_basex + 300, button_basey, 250, 50);
            basic_buttons[2] = new Button(buttonLocPic, button_basex, button_basey + 60, 250, 50);
            basic_buttons[3] = new Button(buttonLocPic, button_basex + 300, button_basey + 60, 250, 50);

            skill_buttons[0] = new Button(buttonLocPic, button_basex - 75, button_basey, 200, 50);
            skill_buttons[1] = new Button(buttonLocPic, button_basex - 75, button_basey + 60, 200, 50);
            skill_buttons[2] = new Button(buttonLocPic, button_basex + 140, button_basey, 200, 50);
            skill_buttons[3] = new Button(buttonLocPic, button_basex + 140, button_basey + 60, 200, 50);
            skill_buttons[4] = new Button(buttonLocPic, button_basex + 350, button_basey, 200, 50);
            skill_buttons[5] = new Button(buttonLocPic, button_basex + 350, button_basey + 60, 200, 50);

            basic_buttons[0].UpdateText("Attack");
            basic_buttons[1].UpdateText("Skills");
            basic_buttons[2].UpdateText("Defend");
            basic_buttons[3].UpdateText("Bribe");


            bossLoc = new Rectangle(LeaveMeAlone.WindowX-300, LeaveMeAlone.WindowY/2 - 150, 200, 200); 
            boss_hp = new Text("", new Vector2(bossLoc.X, bossLoc.Y + 100));
            boss_energy = new Text("", new Vector2(bossLoc.X, bossLoc.Y + 120));


            for (int i = 0; i < 4; i++)
            {
                Text hptext = new Text(msg:"");
                hero_hp.Add(hptext);
            }


            back_button = new Button(Content.Load<Texture2D>("Back"), LeaveMeAlone.WindowX-250, LeaveMeAlone.WindowY-100, 113, 51);


            victory_text = new Text("Victory!\nWe will survive another day!", new Vector2(300, 50), Text.fonts["6809Chargen-24"]);
            defeat_text = new Text("Defeat\nYour friends will be so embarrased with you", new Vector2(300, 50), Text.fonts["6809Chargen-24"]);

            
            info_text = new Text("", new Vector2(200, 50));
            info_counter = 240;


            next_button = new Button(Content.Load<Texture2D>("Next"), LeaveMeAlone.BackgroundRect.Width - 120, LeaveMeAlone.BackgroundRect.Height - 50, 113, 32);

            //---Bribe Stuff---//
            bribe_gold = 0;
            bribe_amounts[0] = new Button(buttonLocPic, button_basex, button_basey, 250, 50);
            bribe_amounts[1] = new Button(buttonLocPic, button_basex + 300, button_basey, 250, 50);
            bribe_amounts[2] = new Button(buttonLocPic, button_basex, button_basey + 60, 250, 50);
            bribe_amounts[3] = new Button(buttonLocPic, button_basex + 300, button_basey + 60, 250, 50);
            for (int i = 0; i < 4; i += 1)
            {
                bribe_amounts[i].UpdateText((Math.Pow(10,i+1)).ToString());
            }

            total_amount = new Button(buttonLocPic, button_basex + 300, button_basey - 60, 200, 50);
            my_amount = new Button(buttonLocPic, button_basex + 50, button_basey - 60, 200, 50);

        }

        public static void Init()
        {
            message = new Text("", new Vector2(0,0), Text.fonts["Arial-12"], Color.Black);

            //Play the Music
            //MediaPlayer.IsRepeatable = true;
            LeaveMeAlone.Menu_Song_Instance.Stop();
            LeaveMeAlone.Battle_Song_Instance.Play();


            victory = false;
            defeat = false;
            haste_check = false;
            enemy_turn = -1;
            state = State.Basic;
            boss.health = boss.max_health;
            boss.energy = boss.max_energy;
            boss_hp.changeMessage(boss.health.ToString() + "/" + boss.max_health.ToString());
            boss_energy.changeMessage(boss.energy.ToString() + "/" + boss.max_energy.ToString());

            for (int i = 0; i < 6; i++)
            {
                check_cooldown[i] = 0;
            }

            for (int i = 0; i < heroes.Count(); i++)
            {
                if (heroes[i] != null)
                {
                    hero_hp[i].changeMessage(heroes[i].health + "/" + heroes[i].max_health);
                }
            }

            foreach (Character hero in heroes)
            {
                Console.WriteLine(hero);
            }

            NewMenu(0);

            total_amount.UpdateText("How Much?: 0");
            my_amount.UpdateText("My Total: " + Resources.gold.ToString());
        }

        public static void Apply_Status(Character affected, Status.Effect_Time effect_time)
        {
            //iterating through the list backwards allows us to properly remove them from the list (it auto-concatenates after every removal)
            //Console.WriteLine("Applying statuses on Character: " + affected.health);
            for (int i = affected.statuses.Count() - 1; i >= 0; i--)
            {
                Status status = affected.statuses[i];
                //If the effect is a one time, increment the counter and move on
                if (effect_time == Status.Effect_Time.Once && status.effect_time == Status.Effect_Time.Once)
                {
                    //Console.WriteLine("Once: "+ status.ToString() + " : " + status.duration_left);
                    //If it's the first time, apply the status affect
                    if (status.duration_left == status.duration)
                    {
                        status.affect(affected);
                    }
                    status.duration_left--;
                    if (status.duration_left == 0)
                    {
                        //reverse the affect and remove the status
                        if (status.reverse_affect != null)
                        {
                            status.reverse_affect(affected);
                        }
                        affected.statuses.Remove(status);
                    }
                }
                
                //If the effect is not one time, do the effect and increment counter
                else if (effect_time == status.effect_time && status.effect_time == Status.Effect_Time.After)
                {
                    //Console.WriteLine("After: " + status.ToString() + " : " + status.duration_left);
                    status.affect(affected);
                    //Whenever the status is triggered, check if the status should be removed                    
                    if (status.duration_left-- == 0)
                    {
                        affected.statuses.Remove(status);
                    }

                }

            }
        }


        public static void Attack(Character caster)
        {
            //targeted_enemy is our target
            //selected_skill is our skill
            if (caster.statuses.Contains(Status.check_stun))
            {
                Console.WriteLine("I'm Stunned!");
                Apply_Status(caster, Status.Effect_Time.Before);
                Apply_Status(caster, Status.Effect_Time.After);
                Apply_Status(caster, Status.Effect_Time.Once);

                //If it's the boss's turn pass to the heroes
                if (enemy_turn == -1)
                {
                    if (boss.statuses.Contains(Status.check_haste) && !haste_check)
                    {
                        //go again
                        left_click = true;
                        state = State.Basic;
                        haste_check = true;
                    }
                    else
                    {
                        enemy_turn = 0;
                        state = State.EnemyTurn;
                        haste_check = false;
                    }
                }
                return;
            }

            if (targeted_enemy >= heroes.Count() || (enemy_turn == -1 && targeted_enemy == -2))
            {
                state = State.Target;
                return;
            }
            //Initiate animation
            caster.attackAnimation();

            //Check if targeted_enemy is within the party size
            


            if (targeted_enemy >= 0)
            {
                //if hero is dead, ignore
                if (heroes[targeted_enemy] == null)
                {
                    state = State.Target;
                    return;
                }
                Apply_Status(caster, Status.Effect_Time.Before);
                caster.cast(selected_skill, heroes[targeted_enemy]);
            }

            

            else if (targeted_enemy == -1)
            {
                Apply_Status(caster, Status.Effect_Time.Before);
                caster.cast(selected_skill);

            }
            //For enemy turns
            else if (targeted_enemy == -2)
            {
                Apply_Status(caster, Status.Effect_Time.Before);
                heroes[enemy_turn].cast(selected_skill, boss);
            }
            //if it's the hero's turn
            if (enemy_turn == -1)
            {
                Console.WriteLine("reducing cooldowns");
                for (int i = 0; i < 6; i++)
                {
                    if (check_cooldown[i] > 0)
                    {
                        check_cooldown[i]--;
                    }
                }
                if (boss.selected_skills.Contains(selected_skill))
                {
                    check_cooldown[boss.selected_skills.IndexOf(selected_skill)] += selected_skill.cooldown;
                }
                
            }

            //apply affects for after the attack
            Apply_Status(caster, Status.Effect_Time.After);

            //check the duration remaining on once effects
            Apply_Status(caster, Status.Effect_Time.Once);

            //Do damage and send state to enemy turn
            //Update texts
            for (int i = 0; i < heroes.Count(); i++)
            {
                if (heroes[i] == null) { continue; }
                hero_hp[i].changeMessage(heroes[i].health.ToString() + "/" + heroes[i].max_health.ToString());
            }
            boss_hp.changeMessage(boss.health.ToString() + "/" + boss.max_health.ToString());
            boss_energy.changeMessage(boss.energy.ToString() + "/" + boss.max_energy.ToString());

            //update the state to pass the turn to enemies
            if (enemy_turn == -1)
            {
                if (boss.statuses.Contains(Status.check_haste) && !haste_check)
                {
                    //go again
                    left_click = true;
                    state = State.Basic;
                    haste_check = true;
                }
                else
                {
                    enemy_turn = 0;
                    state = State.EnemyTurn;
                    haste_check = false;
                }
            }
            //Check after the Boss goes
            CheckVictoryDefeat();
        }

        /*
         * Targetting menu, it will return the int of the target or -1 if no target
         */
        public static int Target()
        {
            int selectLocX = Mouse.GetState().X;
            int selectLocY = Mouse.GetState().Y;

            bool any_target = false;

            for (int i = 0; i < heroes.Count(); i++)
            {
                if (heroLoc[i].Contains(selectLocX, selectLocY) && heroes[i] != null)
                {
                    hovered_enemy = i;
                    any_target = true;
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        return hovered_enemy;
                    }
                }
            }
            if (bossLoc.Contains(selectLocX, selectLocY))
            {
                hovered_enemy = -2;
                any_target = true;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    return hovered_enemy;
                }

            }
            if (!any_target)
            {
                //Console.WriteLine("None found");
                hovered_enemy = -1;
            }
            //return no target if no target has been selected
            return -1;
        }
        public static void CheckVictoryDefeat()
        {
            victory = true;
            defeat = false;
            Character hero;
            for (int i = 0; i < heroes.Count; i++)
            {
                hero = heroes[i];
                if (hero == null)
                {
                    continue;
                }
                if (hero.health > 0)
                {
                    victory = false;
                }
                else
                {
                    Console.WriteLine("Removing Enemy: " + i + " At health: " + hero.health);
                    Resources.gold += heroes[i].gold;
                    Resources.exp += heroes[i].exp;     
                    heroes[i] = null;

                    //Reward the boss
                }
            }
            if (boss.health <= 0)
            {
                defeat = true;
            }
            if (victory || defeat)
            {
                boss.level = Resources.get_level(Resources.exp);
                LeaveMeAlone.Battle_Song_Instance.Stop();
                state = State.Endgame;
            }
        }

        private static void NewMenu(int menu)
        {
            left_click = true;
            if (victory)
            {
                return;
            }
            switch (menu)
            {
                case 0:
                    state = 0;
                    break;
                case 1:
                    //Skills Menu
                    state = State.Skills;
                    for (int i = 0; i < 6; i++)
                    {
                        try
                        {
                            skill_buttons[i].UpdateText(boss.selected_skills[i].name);
                        }
                        catch
                        {
                            skill_buttons[i].UpdateText("NONE");
                        }
                    }
                    break;
                case 2:
                    //Bribe Menu
                    state = State.Bribe;
                    break;
                default:
                    //When will we need this?
                    {
                        state = State.Basic;
                    }
                    break;
            }
        }

        public static LeaveMeAlone.GameState Update(GameTime gametime)
        {
            //Keyboard.GetState();
            //If the mouse is released we can continue taking new input
            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                left_click = false;
            }
            if (Mouse.GetState().RightButton == ButtonState.Released)
            {
                right_click = false;
            }
            switch (state)
            {
                case State.Basic:
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && !left_click)
                    {
                        int selectLocX = Mouse.GetState().X;
                        int selectLocY = Mouse.GetState().Y;
                        if (basic_buttons[1].Intersects(selectLocX, selectLocY))
                        {
                            //Go toSkill menu
                            NewMenu(1);
                        }
                        else if (basic_buttons[3].Intersects(selectLocX, selectLocY))
                        {
                            //Go to Bribe menu
                            NewMenu(2);
                        }
                        else if (basic_buttons[0].Intersects(selectLocX, selectLocY))
                        {
                            //TODO: need a way to select basic attack
                            selected_skill = boss.basic_attack;

                            state = State.Target;
                        }
                        else if (basic_buttons[2].Intersects(selectLocX, selectLocY))
                        {
                            //TODO: need a way to select taunt
                            selected_skill = boss.defend;
                            targeted_enemy = -1; //Don't need this
                            state = State.Attack;
                        }
                    }
                    break;
                case State.Skills:
                    //Skill Selection
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && !left_click)
                    {
                        int selectLocX = Mouse.GetState().X;
                        int selectLocY = Mouse.GetState().Y;

                        message.changeMessage(selectLocX + ", " + selectLocY);
                        for (int i = 0; i < 6; i++)
                        {
                            if (skill_buttons[i].Intersects(selectLocX, selectLocY))
                            {
                                try
                                {
                                    selected_skill = boss.selected_skills[i];
                                    //check cooldown
                                    if (check_cooldown[i] > 0)
                                    {
                                        info_text.changeMessage("Can't use skill, wait for cooldown:" + check_cooldown[i]);
                                        continue;
                                    }
                                    //check mana_cost
                                    if (selected_skill.energy > boss.energy)
                                    {
                                        info_text.changeMessage("Not Enough Energy!");
                                        continue;
                                    }
                                    if (selected_skill.target == Skill.Target.Single)
                                    {
                                        state = State.Target;
                                    }
                                    else
                                    {
                                        state = State.Attack;
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (back_button.Intersects(selectLocX, selectLocY))
                        {
                            NewMenu(0);
                            state = 0;
                        }

                    }
                    break;
                case State.Bribe:
                    //Bribe Stuff
                    
                    if (Mouse.GetState().RightButton == ButtonState.Pressed && !right_click)
                    {
                        int selectLocX = Mouse.GetState().X;
                        int selectLocY = Mouse.GetState().Y;
                        for (int i = 0; i < 4; i++)
                        {
                            if (bribe_amounts[i].Intersects(selectLocX, selectLocY))
                            {
                                bribe_gold -= (int)Math.Pow(10, i + 1);
                                total_amount.UpdateText("How Much?: " + bribe_gold.ToString());
                                right_click = true;
                            }
                        }
                    }
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && !left_click)
                    {
                        int selectLocX = Mouse.GetState().X;
                        int selectLocY = Mouse.GetState().Y;
                        for (int i = 0; i < 4; i++)
                        {
                            if (bribe_amounts[i].Intersects(selectLocX, selectLocY))
                            {
                                bribe_gold += (int) Math.Pow(10, i+1);
                                total_amount.UpdateText("How Much?: " + bribe_gold.ToString());
                                left_click = true;
                            }
                        }
                        if (back_button.Intersects(selectLocX, selectLocY))
                        {
                            NewMenu(0);
                            state = 0;
                            bribe_gold = 0;
                            total_amount.UpdateText("How Much?: 0");
                        }
                    }
                        //Send bribe target at enemy
                    targeted_enemy = Target();
                    if (targeted_enemy >= 0)
                    {
                        if (heroes[targeted_enemy].gold <= bribe_gold && Resources.gold >= bribe_gold)
                        {
                            //remove hero
                            heroes[targeted_enemy] = null;
                            Resources.gold -= bribe_gold;
                            my_amount.UpdateText("My Total: " + Resources.gold);
                            CheckVictoryDefeat();
                        }
                        else
                        {
                            NewMenu(0);
                            state = 0;
                            hovered_enemy = -1;

                        }
                        bribe_gold = 0;
                        total_amount.UpdateText("How Much?: 0");
                    }
                    break;
                case State.Target:
                    //Targetting

                    //highlighted needs to be separate from targeted enemy because target ensure that we have clicked on something
                    targeted_enemy = Target();
                    if (targeted_enemy != -1)
                    {

                        state = State.Attack;
                        hovered_enemy = -1;
                    }
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        int selectLocX = Mouse.GetState().X;
                        int selectLocY = Mouse.GetState().Y;

                        if (back_button.Intersects(selectLocX, selectLocY))
                        {
                            NewMenu(0);
                        }
                    }

                    break;
                case State.Attack:
                    //Attacking
                    Attack(boss);
                    break;
                case State.Endgame:
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && !left_click)
                    {
                        int selectLocX = Mouse.GetState().X;
                        int selectLocY = Mouse.GetState().Y;
                        if (next_button.Intersects(selectLocX, selectLocY))
                        {
                            //Clear the boss's stats
                            foreach (Status status in boss.statuses)
                            {
                                if (status.effect_time == Status.Effect_Time.Once && status.reverse_affect != null)
                                {
                                    status.reverse_affect(boss);
                                }
                            }
                            boss.statuses.Clear();
                            if (victory)
                            {
                                if (LairManager.EndOfGame)
                                {
                                    return LeaveMeAlone.GameState.Credits;
                                }
                                //Do next battle
                                //Go to next (Upgrade) menu
                                PartyManager.PartyNum++;
                                //MainMenu.init();
                                //heroLoc.Clear();
                                victory = false;
                                UpgradeMenu.rerollRooms();
                                LairManager.Init();
                                return LeaveMeAlone.GameState.Lair;
                            }
                            else if (defeat)
                            {
                                //Restart battle
                                //heroLoc.Clear();
                                MainMenu.init(false);
                                return LeaveMeAlone.GameState.Main;

                            }
                        }
                    }
                    break;
                case State.EnemyTurn:
                    //Enemy Turn
                    //Wait to allow the user to see what's happening
                    if (enemy_attack_delay > 0)
                    {
                        enemy_attack_delay--;
                        break;
                    }
                    if (enemy_turn >= heroes.Count())
                    {
                        state = State.Basic;
                        NewMenu(0);
                        enemy_turn = -1;
                        targeted_enemy = -1;
                        CheckVictoryDefeat();
                        break;
                    }

                    Character enemy = heroes[enemy_turn];
                    if (enemy == null)
                    {
                        enemy_turn++;
                        break;
                    }
                    enemy_attack_delay = 60;

                    //AI occurs
                    var pair = enemy.Think();
                    selected_skill = pair.Key;
                    targeted_enemy = pair.Value;
                    Attack(enemy);

                    //Check if this enemy has haste, and check if a hasted enemy has already attacked
                    if (enemy.statuses.Contains(Status.check_haste) && !haste_check)
                    {
                        haste_check = true;
                        CheckVictoryDefeat();
                        break;
                    }
                    else //pass the turn to the next character
                    {
                        haste_check = false;
                        enemy_turn++;
                    }
                    //Check if end of enemy turn;
                    if (enemy_turn >= heroes.Count())
                    {
                        state = State.Basic;
                        NewMenu(0);
                        enemy_turn = -1;
                        targeted_enemy = -1;
                    }
                    //Check after each Enemy
                    CheckVictoryDefeat();
                    break;
            }

            for (int i = 0; i < heroes.Count(); i++)
            {
                if (heroes[i] == null) { continue; }
                heroes[i].Update(gametime);
            }
            boss.Update(gametime);
            boss_hp.changeMessage(BattleManager.boss.health.ToString() + "/" + BattleManager.boss.max_health.ToString());
            boss_energy.changeMessage(BattleManager.boss.energy.ToString() + "/" + BattleManager.boss.max_energy.ToString());
            return LeaveMeAlone.GameState.Battle;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            //Do Background drawing

            spriteBatch.Draw(bkgd, new Rectangle(-450, -100, 2000, 1086), Color.White);
            //Draw Heroes
            //Console.WriteLine("State: " + state.ToString() + " Hovered Enemy: "+hovered_enemy);
            for (int i = 0; i < heroes.Count(); i++)
            {
                try
                {
                    if (i == hovered_enemy)
                    {
                        heroes[i].Draw(spriteBatch, Color.Violet);
                        if (state == State.Target || state == State.Bribe)
                        {
                            target_text.Draw(spriteBatch);
                            spriteBatch.Draw(targeter, new Vector2(heroLoc[i].Location.X + 20, heroLoc[i].Location.Y), Color.Red);
                        }
                    }
                    else
                    {
                        heroes[i].Draw(spriteBatch, Color.White);
                        if (state == State.Target || state == State.Bribe)
                        {
                            target_text.Draw(spriteBatch);
                            spriteBatch.Draw(targeter, new Vector2(heroLoc[i].Location.X + 20, heroLoc[i].Location.Y), Color.Black);
                        }
                    }
                    hero_hp[i].Draw(spriteBatch, new Vector2(heroLoc[i].Location.X, heroLoc[i].Location.Y + 30));

                    if (!heroes[i].damage_text.message.Equals(""))
                    {
                        if (heroes[i].damage_counter-- >= 0)
                        {
                            heroes[i].damage_text.Draw(spriteBatch,new Vector2(heroLoc[i].Location.X, heroLoc[i].Location.Y - 20 + heroes[i].damage_counter / 3), Color.AntiqueWhite);
                        }
                        else
                        {
                            heroes[i].damage_counter = 150;
                            heroes[i].damage_text.changeMessage("");
                        }
                    }
                    else
                    {
                        //Console.WriteLine("This is what is there: " + heroes[i].damage_text.message);
                    }
                }
                catch (NullReferenceException)
                {
                    //dead/KO animation
                }

            }

            //Draw Boss
            boss.Draw(spriteBatch, Color.White);
            boss_hp.Draw(spriteBatch);
            boss_energy.Draw(spriteBatch);
            if (!boss.damage_text.message.Equals(""))
            {
                if (boss.damage_counter-- >= 0)
                {
                    boss.damage_text.Draw(spriteBatch, new Vector2(bossLoc.Location.X, bossLoc.Location.Y - 20 + boss.damage_counter / 3), Color.AntiqueWhite);
                }
                else
                {
                    boss.damage_counter = 150;
                    boss.damage_text.changeMessage("");
                }
            }

            if (info_counter > 0 && !info_text.message.Equals(""))
            {
                info_text.Draw(spriteBatch, new Vector2(200, 50));
                info_counter--;
            }
            else
            {
                info_counter = 240;
                info_text.changeMessage("");
            }

            //Check if we have victory
            if (victory)
            {
                victory_text.Draw(spriteBatch);
                next_button.Draw(spriteBatch);
                return;
            }
            else if (defeat)
            {
                defeat_text.Draw(spriteBatch);
                next_button.Draw(spriteBatch);
                return;
            }

            //Draw Buttons
            if (state == State.Basic)
            {
                for (int i = 0; i < 4; i++)
                {
                    basic_buttons[i].Draw(spriteBatch);
                }


            }
            else if (state == State.Skills)
            {
                for (int i = 0; i < 6; i++)
                {
                    skill_buttons[i].Draw(spriteBatch);
                }
            }

            else if (state == State.Bribe)
            {
                for (int i = 0; i < 4; i++)
                {
                    bribe_amounts[i].Draw(spriteBatch);
                }
                total_amount.Draw(spriteBatch);
                my_amount.Draw(spriteBatch);
            }

            if (state == State.Skills || state == State.Bribe || state == State.Target)
            {
                back_button.Draw(spriteBatch);
            }


            message.Draw(spriteBatch);
        }

        public static void bossDefaultPosition()
        {
            //boss.sPosition = new Vector2(LeaveMeAlone.WindowX-260, LeaveMeAlone.WindowY/2);
        }
        /*
        public static void setHeroesPosition()
        {
            for (int i = 0; i < heroes.Count(); i++)
            {
                if (heroes[i] != null)
                {
                    heroLoc[i] = new Rectangle(herobase.X - 50*i + 150, herobase.Y + 100*i, 150, 100);
                    //heroes[i].sPosition = new Vector2(heroLoc[i].X + 20, heroLoc[i].Y);

                }
            }
        }*/
    }
}

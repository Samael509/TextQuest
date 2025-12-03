using System;
using System.Collections.Generic;

namespace TextQuest
{
    class Program
    {
        static Random random = new Random();
        static int coins = 0;
        static bool hasFoundExit = false;

        static bool devMode = false;
        static bool hasPotion = false;
        static bool hasShield = false;
        static bool hasAmulet = false;
        static bool bonusCoinsUsed = false;

        static private int playerHp = 100;
        static private int playerDamage = 15;
        static int armorHpBonus = 0;
        static int swordDamageBonus = 0;

        static Enemy skeletonEnemy = new Enemy("скелет", 30, 10, 10);
        static Enemy troll = new Enemy("троль", 20, 15, 30);
        static Enemy archer = new Enemy("лучник", 20, 10, 35);
        static Enemy ogre = new Enemy("огр", 70, 30, 80, true);
        static Enemy goblin = new Enemy("гоблин", 30, 8, 15);
        static Enemy kobold = new Enemy("кобольд", 25, 6, 12);
        static Enemy zombie = new Enemy("зомби", 30, 15, 20);

        static List<(string question, string answer)> riddles = new List<(string, string)>
        {
            ("Что всегда идет вперед, но никогда не идет назад?", "время"),
            ("Что можно поймать, но нельзя удержать?", "простуду"),
            ("Что имеет ключ, но не замок?", "клавиатура"),
            ("Что растет вверх ногами?", "корень"),
            ("Что можно разбить, не прикасаясь?", "тишину"),
            ("Что у всех на виду, но никто не замечает?", "воздух"),
            ("Что можно сломать, не прикасаясь?", "обещание"),
            ("Что бывает в каждом доме, но всегда остается невидимым?", "время")
        };

        static void PotionEffect()
        {
            int healAmount = 50;
            playerHp += healAmount;
            if (playerHp > 100) playerHp = 100;
            Console.WriteLine($"вы использовали зелье! здоровье увеличено на {healAmount}. текущее здоровье: {playerHp}");
        }

        static List<(string name, int price, Action effect)> shopItems = new List<(string, int, Action)>
        {
            ("щит (уменьшает урон)", 30, () => { hasShield = true; Console.WriteLine("вы купили щит!"); }),
            ("амулет (уменьшает штраф призрака)", 30, () => { hasAmulet = true; Console.WriteLine("вы купили амулет!"); }),
            ("бонус монет +20 (можно использовать один раз)", 10, () => {
                if (!bonusCoinsUsed)
                {
                    coins += 20;
                    bonusCoinsUsed = true;
                    Console.WriteLine("вы получили +20 монет!");
                }
                else
                {
                    Console.WriteLine("этот бонус уже использован.");
                }
            }),
            ("меч +5 урона", 50, () => { swordDamageBonus += 5; Console.WriteLine("вы экипировали меч! урон увеличен на 5."); }),
            ("броня +50 HP", 50, () => { armorHpBonus += 50; playerHp += 50; Console.WriteLine("вы надели броню! ХП увеличено на 50."); }),
            ("зелье HP +50", 30, () => { hasPotion = true; Console.WriteLine("вы купили зелье!"); }),
        };

        static void Main(string[] args)
        {
            Console.WriteLine("добро пожаловать в лабиринт!");

            while (true)
            {
                if (coins >= 500)
                {
                    hasFoundExit = true;
                    break;
                }

                Console.WriteLine($"\nтекущие монеты: {coins}, здоровье: {playerHp}");
                Console.WriteLine("что вы хотите сделать? (1 - влево/ 2 - вправо/ 3 - прямо/ 4 - торговец)/ 5 - сундук (DEV)/ 6 - vic (DEV)");
                string choice = Console.ReadLine().ToLower();

                if (choice == "dev mode" || choice == "dv")
                {
                    devMode = true;
                    Console.WriteLine("DEV MODE: ON");
                    playerHp = 99999;
                    playerDamage = 999999;
                    hasAmulet = true;
                    hasShield = true;
                    continue;
                }
                if (choice == "give gold")
                {
                    if (!devMode)
                    {
                        Console.WriteLine("ERROR: AN ACCESS FLAG IS NEEDED");
                        continue;
                    }
                    coins += 300;
                }
                if (choice == "5")
                {
                    OpenChest();
                    continue;
                }
                //else if (choice == "монеты")
                //{
                //    Console.WriteLine($"у вас сейчас {coins} монет.");
                //    continue;
                //}
                else if (choice == "5")
                {
                    VisitTrader();
                    continue;
                }
                else if (choice == "6")
                {
                    DevWin();
                    continue;
                }

                switch (choice)
                {
                    case "1":
                    case "2":
                    case "3":
                        MoveAndCheckEncounter();
                        break;
                    default:
                        Console.WriteLine("пожалуйста, выберите один из вариантов: 1 - влево/ 2 - вправо/ 3 - прямо/ 4 - торговец/ 5 - сундук (DEV)/ 6 - vic (DEV)");
                        break;
                }
            }

            Console.WriteLine("вы нашли выход из лабиринта! победа");
            Console.WriteLine($"всего собранных монет: {coins}");
        }

        static void OpenChest()
        {
            if (!devMode)
            {
                Console.WriteLine("ERROR: AN ACCESS FLAG IS NEEDED");
                return;
            }

            Console.WriteLine("вы нашли сундук! что внутри?");
            int coinsFound = random.Next(20, 100);
            coins += coinsFound;
            Console.WriteLine($"в сундуке вы нашли {coinsFound} монет");

        }

        static void DevWin()
        {
            if (!devMode)
            {
                Console.WriteLine("ERROR: AN ACCESS FLAG IS NEEDED");
                return;
            }
            Console.WriteLine("победа изи");
            Environment.Exit(0);
        }

        static void MoveAndCheckEncounter()
        {
            int roll = random.Next(0, 100);

            if (roll < 3) // 3%
            {
                BattleWithEnemy(ogre);
                return;
            }
            else if (roll < 8) // 5%
            {
                BattleWithEnemy(troll);
                return;
            }
            else if (roll < 15) // 7%
            {
                BattleWithEnemy(archer);
                return;
            }
            else if (roll < 30) // 15%
            {
                BattleWithEnemy(kobold);
                return;
            }
            else if (roll < 50) // 20%
            {
                BattleWithEnemy(goblin);
                return;
            }
            else if (roll < 70) // 20%
            {
                BattleWithEnemy(zombie);
                return;
            }
            else if (roll < 85) // 15%
            {
                BattleWithEnemy(skeletonEnemy);
                return;
            }
            else if (roll < 95) // 10%
            {
                OpenChest();
                return;
            }
            else // 5%
            {
                EncounterGhost();
                return;
            }
        }

        static void BattleWithEnemy(Enemy enemyTemplate)
        {
            Enemy enemy = new Enemy(enemyTemplate.Name, enemyTemplate.Hp, enemyTemplate.Damage, enemyTemplate.Reward);
            bool playerDefending = false;
            bool playerStunned = false;

            if (enemy.Name == "огр")
            {
                if (random.Next(0, 100) < 15)
                {
                    enemy.IsStunning = true;
                    Console.WriteLine("огр оглушил вас! Следующий ход пропущен.");
                }
            }

            Console.WriteLine($"на вас напал {enemy.Name}! защищайся");

            while (enemy.Hp > 0 && playerHp > 0)
            {
                if (playerStunned)
                {
                    Console.WriteLine("звёзды в глазах летают");
                    playerStunned = false;
                }
                else
                {
                    Console.WriteLine("Что делать? (1 - атаковать / 2 - защищаться / 3 - зелье)");
                    string action = Console.ReadLine().ToLower();

                    switch (action)
                    {
                        case "1":
                        case "атаковать":
                            {
                                int damageDealt = playerDamage + swordDamageBonus;
                                enemy.Hp -= damageDealt;
                                if (enemy.Hp < 0)
                                    enemy.Hp = 0;
                                Console.WriteLine($"вы нанесли {damageDealt} урона. остаток HP {enemy.Name}: {enemy.Hp}");
                                break;
                            }
                        case "2":
                        case "защищаться":
                            {
                                playerDefending = true;
                                Console.WriteLine("вы защищаетесь на этот ход");
                                break;
                            }
                        case "3":
                        case "зелье":
                            {
                                if (hasPotion)
                                {
                                    PotionEffect();
                                    hasPotion = false;
                                }
                                else
                                {
                                    Console.WriteLine("у вас нет зелья для использования.");
                                }
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("некорректный выбор, пропускаем ход.");
                                break;
                            }
                    }
                }

                if (enemy.Hp <= 0)
                {
                    Console.WriteLine($"вы победили {enemy.Name}! +{enemy.Reward} монет");
                    coins += enemy.Reward;
                    return;
                }

                if (enemy.Name == "огр" && enemy.IsStunning)
                {
                    Console.WriteLine("огр оглушил вас! следующий ход пропущен.");
                    playerStunned = true;
                    enemy.IsStunning = false;
                }

                int damageTaken = enemy.Damage;

                if (enemy.Name == "кобольд")
                {
                    if (random.Next(0, 100) < 5)
                    {
                        Console.WriteLine($"{enemy.Name} быстрая тварь. Промах");
                        damageTaken = 0;
                    }
                }

                if (playerDefending)
                {
                    if (hasShield)
                    {
                        damageTaken /= 2;
                    }
                    else
                    {
                        damageTaken = (int)(damageTaken * 0.97);
                    }
                }

                playerHp -= damageTaken;
                if (playerHp < 0)
                    playerHp = 0;
                Console.WriteLine($"{enemy.Name} атакует: {damageTaken}. ваше HP: {playerHp}");

                if (playerHp <= 0)
                {
                    Console.WriteLine("игра окончена");
                    Environment.Exit(0);
                }

                playerDefending = false;
            }
        }

        static void EncounterGhost()
        {
            Console.WriteLine("внезапно перед вами появляется дух, он загадывает загадку.");

            var riddle = riddles[random.Next(riddles.Count)];

            bool answeredCorrectly = false;
            for (int attempt = 1; attempt <= 2; attempt++)
            {
                Console.WriteLine($"загадка: {riddle.question}");
                string userAnswer = Console.ReadLine().ToLower();

                if (userAnswer.Contains(riddle.answer))
                {
                    Console.WriteLine("правильно! дух пропустил вас. +10 монет");
                    coins += 10;
                    answeredCorrectly = true;
                    break;
                }
                else
                {
                    if (attempt == 1)
                    {
                        Console.WriteLine("неправильно! попробуйте еще раз.");
                    }
                }
            }

            if (!answeredCorrectly)
            {
                int penalty = hasAmulet ? 5 : 10;
                Console.WriteLine($"вы неправильно ответили оба раза. дух забирает ваши монеты (-{penalty}) и пропускает дальше.");
                coins -= penalty;
                if (coins < 0) coins = 0;
            }
        }

        static void VisitTrader()
        {
            Console.WriteLine("привет, что-то интересует?");
            for (int i = 0; i < shopItems.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {shopItems[i].name} - {shopItems[i].price} монет");
            }

            Console.WriteLine("введите номер товара, чтобы купить, или 0 для выхода:");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice > 0 && choice <= shopItems.Count)
                {
                    var item = shopItems[choice - 1];
                    if (coins >= item.price)
                    {
                        coins -= item.price;
                        item.effect();
                    }
                    else
                    {
                        Console.WriteLine("у вас недостаточно монет для покупки этого предмета.");
                    }
                }
            }
        }
    }
}
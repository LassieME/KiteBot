﻿using System;
using System.Collections.Generic;

namespace KiteBot
{
    //Class returns string for users, making a random pizza from a toppings list
    public class KitePizza
    {
        Random randomSeed = new Random(DateTime.Now.Millisecond);

        public string ParsePizza(string userName, string message)
        {
            List<string> pizzaToppings = new List<string>();

            if (userName.ToLower().Contains("ionic") && !message.ToLower().Contains("opt-out"))
            {
                pizzaToppings.AddRange(new string[] {"Mayonnaise", "Squid", "Raw Tuna", "Raw Salmon", "Avocado","Squid Ink",
                                                      "Broccoli", "Shrimp", "Teriyaki Chicken", "Bonito Flakes", "Hot Sake",
                                                      "Soft Tofu", "Sushi Rice", "Nori", "Corn", "Snow Peas", "Bamboo Shoots",
                                                      "Potato", "Onion"});
            }

            else
                pizzaToppings.AddRange(new string[] {"Extra Cheese", "Pepperoni", "Sausage", "Chicken", "Ham", "Canadian Bacon",
                                                         "Bacon", "Green Peppers", "Black Olives", "White Onion", "Red Onions", "Diced Tomatoes",
                                                         "Spinach", "Roasted Red Peppers", "Sun Dried Tomato", "Pineapple", "Italian Sausage",
                                                         "Red Onion", "Green Chile", "Basil", "Mayonnaise", "Mushrooms", "Beef"});

            int numberOfToppings = randomSeed.Next(2, 7);//2 is 3, 7 is 8

            string buildThisPizza = "USER you should put these things in the pizza: ";

            for (int i = 0; i <= numberOfToppings; i++)
            {
                int j = randomSeed.Next(0, pizzaToppings.Count);
                buildThisPizza += pizzaToppings[j];
                pizzaToppings.Remove(pizzaToppings[j]);

                if (i == numberOfToppings)
                {
                    buildThisPizza += ".";
                }

                else
                {
                    buildThisPizza += ", ";
                }
            }

            return (buildThisPizza.Replace("USER", userName));
        }

    }
}

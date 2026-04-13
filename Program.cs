using System;
using Lab4.Task2_Mediator;
using Lab4.Task5_Memento;
using Lab4.Task1_ChainOfResponsibility;
using Lab4.Task3_Observer;
namespace Lab4
{
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Лабораторна робота №4 — Поведінкові шаблони");

            bool running = true;
            while (running)
            {
                Console.WriteLine("\nОберіть завдання для демонстрації:");
                Console.WriteLine("  1 — Ланцюжок відповідальностей (інтерактивне меню)");
                Console.WriteLine("  2 — Посередник (Аеропорт)");
                Console.WriteLine("  3 — Спостерігач (LightHTML + EventListener)");
                Console.WriteLine("  4 — Стратегія (LightImage)");
                Console.WriteLine("  5 — Мементо (Текстовий редактор)");
                Console.WriteLine("  0 — Вийти");
                Console.Write("\nВаш вибір: ");

                string input = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (input.Trim())
                {
                    case "1": DemoTask1(); break;
                    case "2": DemoTask2(); break;
                    case "3": DemoTask3(); break;
                    case "5": DemoTask5(); break;
                    case "0": running = false; break;
                    default:  Console.WriteLine("Невірний вибір."); break;
                }
            }

            Console.WriteLine("\nДо побачення!");
        }
        static void DemoTask1()
        {
            Console.WriteLine("  ЗАВДАННЯ 1: Ланцюжок відповідальностей");
            var menu = new Task1_ChainOfResponsibility.SupportMenu();
            menu.Run();
        }
        static void DemoTask2()
        {
            Console.WriteLine("  ЗАВДАННЯ 2: Посередник — Аеропорт");

            var centre = new CommandCentre();

            var runway1 = new Runway("R-01", centre);
            var runway2 = new Runway("R-02", centre);

            var flight1 = new Aircraft("UA-101", centre);
            var flight2 = new Aircraft("PS-202", centre);
            var flight3 = new Aircraft("LH-303", centre);

            Console.WriteLine("\n--- Сценарій 1: два запити на посадку ---");
            flight1.RequestLanding();   
            flight2.RequestLanding();  

            Console.WriteLine("\n--- Сценарій 2: третій літак змушений чекати ---");
            flight3.RequestLanding();   

            Console.WriteLine("\n--- Сценарій 3: смуга R-01 звільнилась ---");
            runway1.Release();         

            Console.WriteLine("\n--- Сценарій 4: зліт ---");
            flight1.RequestTakeoff();   

            Console.WriteLine("\n--- Смуга R-02 звільнилась ---");
            runway2.Release();
        }
      
        static void DemoTask3()
        {
            Console.WriteLine("  ЗАВДАННЯ 3: Спостерігач — LightHTML EventListener");

            var div = new LightElement("div");
            var button = new LightElement("button", isBlock: false);
            button.AddChild(new LightText("Натисни мене"));
            div.AddChild(button);

            var h1 = new LightElement("h1", isBlock: false);
            h1.AddChild(new LightText("Заголовок сторінки"));

            Task3_Observer.EventHandler clickHandler1 = (evt, el) =>
                Console.WriteLine($" Handler 1 Клік на <{el.TagName}>! Подія: {evt}");

            Task3_Observer.EventHandler clickHandler2 = (evt, el) =>
                Console.WriteLine($" [Handler 2] Ще один обробник кліку на <{el.TagName}>!");

            Task3_Observer.EventHandler mouseoverHandler = (evt, el) =>
                Console.WriteLine($" [Handler] Миша над <{el.TagName}>! Подія: {evt}");

            Task3_Observer.EventHandler focusHandler = (evt, el) =>
                Console.WriteLine($" [Handler] Фокус на <{el.TagName}>!");

            button.AddEventListener("click", clickHandler1);
            button.AddEventListener("click", clickHandler2);
            button.AddEventListener("mouseover", mouseoverHandler);
            h1.AddEventListener("focus", focusHandler);

            Console.WriteLine("\n--- Симулюємо події ---");
            button.DispatchEvent("click");
            button.DispatchEvent("mouseover");
            button.DispatchEvent("keydown");  

            h1.DispatchEvent("focus");

            Console.WriteLine("\n--- Видаляємо один обробник click ---");
            button.RemoveEventListener("click", clickHandler1);
            button.DispatchEvent("click");     

            Console.WriteLine("\n--- HTML-структура ---");
            Console.WriteLine(div.OuterHtml());
            Console.WriteLine(h1.OuterHtml());
        }
        static void DemoTask5()
        {
            Console.WriteLine("  ЗАВДАННЯ 5: Мементо — Текстовий редактор");

            var doc    = new TextDocument("Новий документ");
            var editor = new TextEditor(doc);

            editor.PrintDocument();

            Console.WriteLine("\n--- Крок 1: друкуємо перший абзац і зберігаємо ---");
            editor.TypeText("Перший рядок документа. ");
            editor.SaveState();
            editor.PrintDocument();

            Console.WriteLine("\n--- Крок 2: додаємо ще текст і зберігаємо ---");
            editor.TypeText("Другий рядок. Важливий вміст.");
            editor.SaveState();
            editor.PrintDocument();

            Console.WriteLine("\n--- Крок 3: змінюємо заголовок і зберігаємо ---");
            editor.ChangeTitle("Відредагований документ");
            editor.SaveState();
            editor.PrintDocument();

            Console.WriteLine("\n--- Крок 4: робимо «помилкову» зміну (не зберігаємо) ---");
            editor.TypeText(" [ПОМИЛКА: небажаний текст!]");
            editor.PrintDocument();

            Console.WriteLine("\n--- Скасовуємо (Undo) — повернемось до кроку 3 ---");
            editor.Undo();
            editor.PrintDocument();

            Console.WriteLine("\n--- Ще одне Undo — повернемось до кроку 2 ---");
            editor.Undo();
            editor.PrintDocument();

            Console.WriteLine("\n--- Ще одне Undo — повернемось до кроку 1 ---");
            editor.Undo();
            editor.PrintDocument();

            Console.WriteLine("\n--- Ще одне Undo — немає більше знімків ---");
            editor.Undo();

            editor.PrintHistory();
        }
    }
}
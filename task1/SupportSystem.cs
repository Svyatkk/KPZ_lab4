using System;

namespace Lab4.Task1_ChainOfResponsibility
{
    public abstract class SupportHandler
    {
        protected SupportHandler? _nextHandler;

        public SupportHandler SetNext(SupportHandler handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public abstract bool Handle(SupportRequest request);
    }

    public class SupportRequest
    {
        public string Category { get; set; } = ""; 
        public string Severity { get; set; } = "";  
        public string SubIssue { get; set; } = "";
    }
    
    public class GeneralSupportHandler : SupportHandler
    {
        public override bool Handle(SupportRequest request)
        {
            if (request.Category == "other")
            {
                Console.WriteLine("\n [Рівень 1 — Загальна підтримка] Ваш запит прийнято. " +
                                  "Оператор загальної підтримки зв'яжеться з вами найближчим часом.");
                return true;
            }

            if (_nextHandler != null)
                return _nextHandler.Handle(request);

            return false;
        }
    }

    public class TechnicalSupportHandler : SupportHandler
    {
        public override bool Handle(SupportRequest request)
        {
            if (request.Category == "technical")
            {
                Console.WriteLine("\n[Рівень 2 — Технічна підтримка] Ваш запит передано технічному спеціалісту. " +
                                  $"Підкатегорія: {request.SubIssue}. Очікуйте відповіді.");
                return true;
            }

            if (_nextHandler != null)
                return _nextHandler.Handle(request);

            return false;
        }
    }

    public class BillingSupportHandler : SupportHandler
    {
        public override bool Handle(SupportRequest request)
        {
            if (request.Category == "billing")
            {
                Console.WriteLine("\n[Рівень 3 — Відділ оплати] Ваш запит щодо оплати прийнято. " +
                                  "Фінансовий спеціаліст перевірить вашу ситуацію.");
                return true;
            }

            if (_nextHandler != null)
                return _nextHandler.Handle(request);

            return false;
        }
    }

    public class AccountSupportHandler : SupportHandler
    {
        public override bool Handle(SupportRequest request)
        {
            if (request.Category == "account")
            {
                string urgency = request.Severity == "high"
                    ? "ТЕРМІНОВО! Ваш обліковий запис заблоковано для безпеки."
                    : "Спеціаліст з облікових записів допоможе вам.";

                Console.WriteLine($"\n [Рівень 4 — Облікові записи] {urgency}");
                return true;
            }

            if (_nextHandler != null)
                return _nextHandler.Handle(request);

            return false;
        }
    }

    public class SupportMenu
    {
        private readonly SupportHandler _chain;

        public SupportMenu()
        {
            var general   = new GeneralSupportHandler();
            var technical = new TechnicalSupportHandler();
            var billing   = new BillingSupportHandler();
            var account   = new AccountSupportHandler();

            general.SetNext(technical).SetNext(billing).SetNext(account);

            _chain = general;
        }

        public void Run()
        {
            Console.WriteLine("  Ласкаво просимо до служби підтримки!  ");

            bool resolved = false;

            while (!resolved)
            {
                var request = new SupportRequest();

                Console.WriteLine("\nОберіть тему вашого звернення:");
                Console.WriteLine("  1 — Технічна проблема");
                Console.WriteLine("  2 — Питання щодо оплати");
                Console.WriteLine("  3 — Проблема з обліковим записом");
                Console.WriteLine("  4 — Інше");
                Console.Write("Ваш вибір: ");
                string catInput = Console.ReadLine() ?? "";

                switch (catInput.Trim())
                {
                    case "1": request.Category = "technical"; break;
                    case "2": request.Category = "billing";   break;
                    case "3": request.Category = "account";   break;
                    case "4": request.Category = "other";     break;
                    default:
                        Console.WriteLine(" Невірний вибір. Спробуйте ще раз.");
                        continue;
                }

                if (request.Category == "technical")
                {
                    Console.WriteLine("\nОберіть тип технічної проблеми:");
                    Console.WriteLine("  1 — Інтернет не працює");
                    Console.WriteLine("  2 — Проблема з додатком");
                    Console.WriteLine("  3 — Пристрій не підключається");
                    Console.Write("Ваш вибір: ");
                    string subInput = Console.ReadLine() ?? "";
                    request.SubIssue = subInput.Trim() switch
                    {
                        "1" => "Інтернет-з'єднання",
                        "2" => "Мобільний додаток",
                        "3" => "Підключення пристрою",
                        _   => "Загальна технічна проблема"
                    };
                }

                if (request.Category == "account")
                {
                    Console.WriteLine("\nОцініть серйозність проблеми:");
                    Console.WriteLine("  1 — Не можу увійти (низька)");
                    Console.WriteLine("  2 — Обліковий запис зламано (висока)");
                    Console.Write("Ваш вибір: ");
                    string sevInput = Console.ReadLine() ?? "";
                    request.Severity = sevInput.Trim() == "2" ? "high" : "low";
                }

                resolved = _chain.Handle(request);

                if (!resolved)
                {
                    Console.WriteLine("\n Не вдалося знайти відповідний рівень підтримки.");
                    Console.WriteLine("    Меню починається заново...");
                }
            }

        }
    }
}
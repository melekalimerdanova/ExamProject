using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace JobAnnouncementSystem
{
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }
    }
    public class InputStreamClosedException : Exception { }

    public class LogRow
    {
        public DateTime Time { get; set; } = DateTime.Now;
        public string Message { get; set; }
        public LogRow() { }
        public LogRow(string message) { Message = message; }
    }

    public class LanguageSkill
    {
        public string? LanguageName { get; set; }
        public string? Level { get; set; }
        public override string ToString() => $"{LanguageName} ({Level})";
    }

    public abstract class User
    {
        private static int lastId = 0;
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public List<string> Notifications { get; set; } = new List<string>();

        protected User() { }

        public void GenerateNewId()
        {
            lastId++;
            this.Id = lastId;
        }

        public static void SetIdCounter(int value)
        {
            if (value > lastId) lastId = value;
        }

        [JsonIgnore]
        public int Age
        {
            get
            {
                int age = DateTime.Now.Year - BirthDate.Year;
                if (DateTime.Now.Date < BirthDate.AddYears(age)) age--;
                return age;
            }
        }

        public static void ValidateBirthDate(DateTime value)
        {
            int age = DateTime.Now.Year - value.Year;
            if (DateTime.Now.Date < value.AddYears(age)) age--;
            if (age < 18) throw new AppException("Yasiniz 18-den kicik ola bilmez!");
            if (value > DateTime.Now) throw new AppException("Dogum tarixi gelecekde ola bilmez!");
        }

        public virtual void Display()
        {
            Console.WriteLine($"#{Id} {Name} {Surname} | Seher: {City} | Tel: {Phone} | Yas: {Age}");
        }
    }

    public class Worker : User
    {
        public List<CV> CVs { get; set; } = new List<CV>();
        public override void Display()
        {
            base.Display(); Console.WriteLine($"  CV Sayi: {CVs.Count}");
        }
    }

    public class Employer : User
    {
        public List<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
        public override void Display()
        {
            base.Display(); Console.WriteLine($"  Elan Sayi: {Vacancies.Count}");
        }
    }

    public class CV
    {
        private static int lastId = 0;
        public int Id { get; set; }
        public string? OwnerUsername { get; set; }
        public string? Speciality { get; set; }
        public string? School { get; set; }
        public double UniAdmissionScore { get; set; }
        public DateTime WorkStartDate { get; set; }
        public DateTime WorkEndDate { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public List<string> Companies { get; set; } = new List<string>();
        public List<LanguageSkill> Languages { get; set; } = new List<LanguageSkill>();
        public bool HonorsDiploma { get; set; }
        public string? GitLink { get; set; }
        public string? LinkedIn { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public CV() { }
        public void GenerateNewId()
        {
            lastId++; Id = lastId;
        }
        public static void SetIdCounter(int value)
        {
            if (value > lastId) lastId = value;
        }

        public void Display()
        {
            Console.WriteLine(new string('-', 40));
            Console.WriteLine($"CV #{Id} | Ixtisas: {Speciality} | Mekteb/Uni: {School}");
            Console.WriteLine($"Qebul Bali: {UniAdmissionScore}");
            Console.WriteLine($"Is Tecrubesi Dovru: {WorkStartDate:dd.MM.yyyy} - {WorkEndDate:dd.MM.yyyy}");
            Console.WriteLine($"Bacariqlar: {string.Join(", ", Skills)}");
            Console.WriteLine($"Islediyi Yerler: {string.Join(", ", Companies)}");
            Console.WriteLine($"Xarici Diller: {string.Join(", ", Languages.Select(l => l.ToString()))}");
            Console.WriteLine($"Ferqlenme Diplomu: {(HonorsDiploma ? "Var" : "Yoxdur")}");
            Console.WriteLine($"GITLINK: {GitLink} | LINKEDIN: {LinkedIn}");
            Console.WriteLine(new string('-', 40));
        }
    }

    public class Vacancy
    {
        private static int lastId = 0;
        private bool? _isActiveManual;

        public int Id { get; set; }
        public string? EmployerUsername { get; set; }
        public string? JobTitle { get; set; }
        public string? CompanyName { get; set; }
        public string? City { get; set; }
        public string? JobType { get; set; }
        public string? ExperienceLevel { get; set; }
        public double MinSalary { get; set; }
        public double MaxSalary { get; set; }
        public string Description { get; set; }
        public List<string> RequiredSkills { get; set; } = new List<string>();
        public DateTime Deadline { get; set; }
        public DateTime PostedDate { get; set; } = DateTime.Now;
        public List<string> Applicants { get; set; } = new List<string>();
        public string? SelectedWorker { get; set; }

        public Vacancy() { }
        public void GenerateNewId()
        {
            lastId++; Id = lastId;
        }
        public static void SetIdCounter(int value)
        {
            if (value > lastId) lastId = value;
        }

        [JsonIgnore]
        public bool IsActive
        {
            get => _isActiveManual ?? (DateTime.Now.Date <= Deadline.Date);
            set => _isActiveManual = value;
        }

        public void Display()
        {
            Console.WriteLine(new string('-', 40));
            Console.WriteLine($"Elan #{Id} {(IsActive ? "" : "[VAXTI BITIB]")}");
            Console.WriteLine($"Vezife: {JobTitle} | Sirket: {CompanyName} | Seher: {City}");
            Console.WriteLine($"Is Formasi: {JobType} | Seviyye: {ExperienceLevel}");
            Console.WriteLine($"Maas: {MinSalary} - {MaxSalary} AZN");
            Console.WriteLine($"Aciqlama: {Description}");
            Console.WriteLine($"Teleb Olunan Bacariqlar: {string.Join(", ", RequiredSkills)}");
            Console.WriteLine($"Son Muraciet: {Deadline:dd.MM.yyyy} | Muraciet Edenlerin Sayi: {Applicants.Count}");
            if (!string.IsNullOrEmpty(SelectedWorker)) Console.WriteLine($"Secilmis Isci: {SelectedWorker}");
            Console.WriteLine(new string('-', 40));
        }
    }

    public delegate void NotificationHandler(string user, string message);

    public static class Database
    {
        const string WORKERS_FILE = "workers.json";
        const string EMPLOYERS_FILE = "employers.json";
        const string LOG_FILE = "log.json";

        public static List<Worker> Workers = new List<Worker>();
        public static List<Employer> Employers = new List<Employer>();
        public static List<LogRow> Logs = new List<LogRow>();

        public static void Load()
        {
            Workers = ReadFromFile<List<Worker>>(WORKERS_FILE) ?? new List<Worker>();
            Employers = ReadFromFile<List<Employer>>(EMPLOYERS_FILE) ?? new List<Employer>();
            Logs = ReadFromFile<List<LogRow>>(LOG_FILE) ?? new List<LogRow>();

            if (Employers.Count == 0)
            {
                var admin = new Employer
                {
                    Username = "admin",
                    Password = "admin123",
                    Name = "Sistem",
                    Surname = "Admini",
                    City = "Baku",
                    Phone = "0500000000",
                    BirthDate = new DateTime(1990, 1, 1)
                };
                admin.GenerateNewId();
                Employers.Add(admin);
            }

            int maxId = 0, maxCV = 0, maxVacancy = 0;
            foreach (var w in Workers)
            {
                if (w.Id > maxId) maxId = w.Id;
                foreach (var cv in w.CVs)
                {
                    if (cv.Id > maxCV) maxCV = cv.Id;
                }
            }
            foreach (var e in Employers)
            {
                if (e.Id > maxId) maxId = e.Id;
                foreach (var v in e.Vacancies)
                {
                    if (v.Id > maxVacancy) maxVacancy = v.Id;
                }
            }
            User.SetIdCounter(maxId);
            CV.SetIdCounter(maxCV);
            Vacancy.SetIdCounter(maxVacancy);
        }

        public static void Save()
        {
            WriteToFile(WORKERS_FILE, Workers);
            WriteToFile(EMPLOYERS_FILE, Employers);
        }

        public static void Log(string message)
        {
            try
            {
                Logs.Add(new LogRow(message)); WriteToFile(LOG_FILE, Logs);
            }
            catch
            {

            }
        }

        static void WriteToFile<T>(string file, T data)
        {
            using var sw = new StreamWriter(file, false, Encoding.UTF8);
            using var jw = new JsonTextWriter(sw) { Formatting = Formatting.Indented };
            new JsonSerializer().Serialize(jw, data);
        }

        static T ReadFromFile<T>(string file)
        {

            if (!File.Exists(file))
            {
                return default;
            }
            try
            {
                using var sr = new StreamReader(file, Encoding.UTF8);
                using var jr = new JsonTextReader(sr);
                return new JsonSerializer().Deserialize<T>(jr);
            }
            catch
            {
                return default;
            }
        }
    }

    public static class InputHelper
    {
        public static string ReadString(string message, bool allowEmpty = false)
        {
            while (true)
            {
                Console.Write(message);
                string? value = Console.ReadLine();
                if (value == null) throw new InputStreamClosedException();
                if (!allowEmpty && string.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine("Bu sahe bos burakila bilmez.");
                    continue;
                }
                return value.Trim();
            }
        }

        public static double ReadDouble(string message, double minValue = double.MinValue)
        {
            while (true)
            {
                if (double.TryParse(ReadString(message), out double n) && n >= minValue)
                    return n;
                Console.WriteLine("Xahis olunur duzgun reqem daxil edin.");
            }
        }

        public static int ReadInt(string message)
        {
            while (true)
            {
                if (int.TryParse(ReadString(message), out int n))
                    return n;
                Console.WriteLine("Xahis olunur duzgun tam eded daxil edin.");
            }
        }

        public static DateTime ReadDate(string message)
        {
            while (true)
            {
                if (DateTime.TryParseExact(ReadString(message), "dd.MM.yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime n))
                    return n;
                Console.WriteLine("Tarixi 'gg.aa.iiii' formatinda daxil edin.");
            }
        }

        public static bool ReadBoolean(string message)
        {
            while (true)
            {
                string d = ReadString(message).ToLower();
                if (d == "b" || d == "beli")
                    return true;
                if (d == "x" || d == "xeyr")
                    return false;
                Console.WriteLine("'b' (beli) ve ya 'x' (xeyr) daxil edin.");
            }
        }

        public static List<string> ReadCommaSeparatedList(string message)
        {
            string d = ReadString(message, allowEmpty: true);
            if (string.IsNullOrWhiteSpace(d))
                return new List<string>();
            return d.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
        }
    }

    class Program
    {
        static event NotificationHandler? OnNewVacancy;
        static event NotificationHandler? OnWorkerSelected;
        static event NotificationHandler? OnNewApplication;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Database.Load();
            SetupNotifications();

            try
            {
                while (true)
                {
                    Console.WriteLine("\n=== IS ELANI SISTEMI ===");
                    Console.WriteLine("1. Daxil ol");
                    Console.WriteLine("2. Isci kimi qeydiyyat");
                    Console.WriteLine("3. Isegoturen kimi qeydiyyat");
                    Console.WriteLine("0. Cixis");
                    Console.Write("Secim edin: ");

                    string? input = Console.ReadLine();
                    if (!int.TryParse(input, out int choice))
                    {
                        Console.WriteLine("Zəhmət olmasa rəqəm daxil edin.");
                        continue;
                    }

                    try
                    {
                        if (choice == 1)
                            Login();
                        else if (choice == 2)
                            RegisterWorker();
                        else if (choice == 3)
                            RegisterEmployer();
                        else if (choice == 0)
                            break;
                        else
                            Console.WriteLine("Yanlis secim.");
                    }
                    catch (AppException ex)
                    {
                        Console.WriteLine("Xeta: " + ex.Message);
                        Database.Log("XETA: " + ex.Message);
                        Database.Save();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Gozlenilmez xeta bas verdi: " + ex.Message);
                        Database.Log("KRITIK XETA: " + ex.Message);
                        Database.Save();
                    }
                }
            }
            catch (InputStreamClosedException) { }
            finally
            {
                Database.Save();
                Console.WriteLine("Melumatlar yaddasa verildi. Sag olun!");
            }
        }

        static string ReadPassword()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                        Console.Write("\b \b");
                    }
                }
                else if (key.KeyChar != '\u0000')
                {
                    sb.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            return sb.ToString();
        }

        static void SetupNotifications()
        {
            OnNewVacancy += (user, msg) =>
            {
                foreach (var w in Database.Workers) w.Notifications.Add(msg);
                Database.Save();
                Database.Log(msg);
            };

            OnWorkerSelected += (username, jobTitle) =>
            {
                var w = Database.Workers.FirstOrDefault(x => x.Username == username);
                if (w != null) w.Notifications.Add($"TEBRIKLER! \"{jobTitle}\" vezifesine secildiniz.");
                Database.Save();
                Database.Log($"{username} '{jobTitle}' vezifesine secildi.");
            };

            OnNewApplication += (username, jobTitle) =>
            {
                var e = Database.Employers.FirstOrDefault(x =>
                    x.Vacancies.Any(v => v.JobTitle == jobTitle && v.Applicants.Contains(username)));
                if (e != null) e.Notifications.Add($"{username} \"{jobTitle}\" vakansiyasina muraciet etdi.");
                Database.Save();
                Database.Log($"{username} '{jobTitle}' vezifesine muraciet etdi.");
            };
        }

        static void RegisterWorker()
        {
            Console.Write("Istifadeci adi: ");
            string? username = Console.ReadLine();

            if (Database.Workers.Any(i => i.Username == username) || Database.Employers.Any(e => e.Username == username))
                throw new AppException("Bu istifadeci adi artiq movcuddur!");

            var w = new Worker { Username = username };
            w.GenerateNewId();

            Console.Write("Sifre: "); w.Password = ReadPassword();
            Console.Write("Ad: "); w.Name = Console.ReadLine();
            Console.Write("Soyad: "); w.Surname = Console.ReadLine();
            Console.Write("Seher: "); w.City = Console.ReadLine();
            Console.Write("Telefon: "); w.Phone = Console.ReadLine();

            Console.Write("Dogum tarihi (gg.aa.iiii): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime birth))
            {
                User.ValidateBirthDate(birth);
                w.BirthDate = birth;
            }
            else
            {
                throw new AppException("Tarix formatı düzgün deyil!");
            }

            Database.Workers.Add(w);
            Database.Save();
            Database.Log($"Yeni isci qeydiyyatdan kecdi: {username}");
            Console.WriteLine("Qeydiyyat tamamlandi!");
        }

        static void RegisterEmployer()
        {
            Console.Write("Istifadeci adi: ");
            string? username = Console.ReadLine();

            if (Database.Workers.Any(i => i.Username == username) || Database.Employers.Any(e => e.Username == username))
                throw new AppException("Bu istifadeci adi artiq movcuddur!");

            var e = new Employer { Username = username };
            e.GenerateNewId();

            Console.Write("Sifre: "); e.Password = ReadPassword();
            Console.Write("Ad: "); e.Name = Console.ReadLine();
            Console.Write("Soyad: "); e.Surname = Console.ReadLine();
            Console.Write("Seher: "); e.City = Console.ReadLine();
            Console.Write("Telefon: "); e.Phone = Console.ReadLine();

            Console.Write("Dogum tarihi (gg.aa.iiii): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime birth))
            {
                User.ValidateBirthDate(birth);
                e.BirthDate = birth;
            }
            else
            {
                throw new AppException("Tarix formatı düzgün deyil!");
            }

            Database.Employers.Add(e);
            Database.Save();
            Database.Log($"Yeni isegoturen qeydiyyatdan kecdi: {username}");
            Console.WriteLine("Isegoturen qeydiyyati tamamlandi!");
        }

        static void Login()
        {
            Console.Write("Istifadeci adi: "); string? username = Console.ReadLine();
            Console.Write("Sifre: "); string? password = ReadPassword();

            var w = Database.Workers.FirstOrDefault(i => i.Username == username);
            if (w != null)
            {
                if (w.Password != password)
                {
                    Console.WriteLine("Sifre sehvdir.");
                    Database.Log($"Uğursuz giriş cəhdi (İşçi): {username}");
                    return;
                }
                Database.Log($"Isci giris etdi: {username}");
                WorkerMenu(w);
                return;
            }

            var e = Database.Employers.FirstOrDefault(x => x.Username == username);
            if (e != null)
            {
                if (e.Password != password)
                {
                    Console.WriteLine("Sifre sehvdir.");
                    Database.Log($"Uğursuz giriş cəhdi (İşəgötürən): {username}");
                    return;
                }
                Database.Log($"Isegoturen giris etdi: {username}");
                EmployerMenu(e);
                return;
            }

            Console.WriteLine("Istifadeci tapilmadi!");
        }

        static void WorkerMenu(Worker w)
        {
            while (true)
            {
                Console.WriteLine($"\nIsci Paneli: {w.Name} {w.Surname}");
                Console.WriteLine("1.CV elave et  2.CV-lerim  3.CV Redakte et  4.CV Sil  5.Vakansiya Axtar  6.Bildirisler  0.Cixis");
                Console.Write("Seciminiz: ");

                if (!int.TryParse(Console.ReadLine(), out int ch))
                {
                    Console.WriteLine("Yanlış seçim.");
                    continue;
                }

                try
                {
                    switch (ch)
                    {
                        case 1:
                            AddCV(w);
                            break;
                        case 2:
                            ShowCVs(w);
                            break;
                        case 3:
                            EditCV(w);
                            break;
                        case 4:
                            DeleteCV(w);
                            break;
                        case 5:
                            ViewVacancies(w);
                            break;
                        case 6:
                            ShowNotifications(w.Notifications);
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Yanlis secim.");
                            break;
                    }
                }
                catch (AppException ex)
                {
                    Console.WriteLine("Xeta: " + ex.Message);
                }
            }
        }

        static void AddCV(Worker w)
        {
            var cv = new CV { OwnerUsername = w.Username };
            cv.GenerateNewId();

            Console.Write("Ixtisas: "); cv.Speciality = Console.ReadLine();
            Console.Write("Mekteb/Universitet: "); cv.School = Console.ReadLine();

            Console.Write("Uni qebul bali (Yoxdursa bos buraxin): ");
            string? scoreInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(scoreInput) && double.TryParse(scoreInput, out double score))
            {
                cv.UniAdmissionScore = score;
            }

            Console.Write("Ise baslama tarixi (gg.aa.iiii): ");
            string? startDateInput = Console.ReadLine();
            Console.Write("Isden cixma tarixi (gg.aa.iiii): ");
            cv.WorkEndDate = DateTime.Parse(Console.ReadLine());

            if (cv.WorkEndDate < cv.WorkStartDate)
                throw new AppException("Bitme tarixi baslama tarixinden evvel ola bilmez.");

            Console.Write("Bacariqlar (Skills) (C#, Java ve s. vergul ile): ");
            cv.Skills = Console.ReadLine().Split(',').Select(s => s.Trim()).ToList();

            Console.Write("Islediyiniz sirketler (vergul ile): ");
            cv.Companies = Console.ReadLine().Split(',').Select(s => s.Trim()).ToList();

            Console.Write("Xarici Diller (Ingilis dili-C1, Rus dili-B2 kimi vergul ile): ");
            cv.Languages = Console.ReadLine().Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s =>
                {
                    var parts = s.Split('-'); return new LanguageSkill
                    {
                        LanguageName = parts[0].Trim(),
                        Level = parts.Length > 1 ? parts[1].Trim() : "Bilinmir"
                    };
                }).ToList();

            Console.Write("Ferqlenme diplomu var? (e/x): ");
            cv.HonorsDiploma = Console.ReadLine().Trim().ToLower() == "e";

            Console.Write("GITLINK: "); cv.GitLink = Console.ReadLine();
            Console.Write("LINKEDIN: "); cv.LinkedIn = Console.ReadLine();

            w.CVs.Add(cv);
            Database.Save();
            Database.Log($"{w.Username} yeni CV yaratdi (ID: {cv.Id})");
            Console.WriteLine("CV ugurla elave olundu!");
        }

        static void ShowCVs(Worker w)
        {
            if (w.CVs.Count == 0) { Console.WriteLine("CV-niz yoxdur."); return; }
            foreach (var cv in w.CVs) cv.Display();
        }

        static void EditCV(Worker w)
        {
            ShowCVs(w);
            if (w.CVs.Count == 0) return;

            Console.Write("Redakte olunacaq CV ID daxil edin: ");
            int id = int.Parse(Console.ReadLine());

            var cv = w.CVs.FirstOrDefault(x => x.Id == id);
            if (cv == null) throw new AppException($"ID={id} olan CV tapilmadi.");

            Console.Write($"Ixtisas ({cv.Speciality}), yenisi (Deyismemek ucun bos buraxin): ");
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) cv.Speciality = input;

            Console.Write($"Mekteb ({cv.School}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) cv.School = input;

            Console.Write($"Qebul bali ({cv.UniAdmissionScore}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input) && double.TryParse(input, out double newScore)) cv.UniAdmissionScore = newScore;

            Console.Write($"Baslama tarixi ({cv.WorkStartDate:dd.MM.yyyy}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) cv.WorkStartDate = DateTime.Parse(input);

            Console.Write($"Bitme tarixi ({cv.WorkEndDate:dd.MM.yyyy}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) cv.WorkEndDate = DateTime.Parse(input);

            Console.Write("Yenilenmis Bacariqlar (Yenilememek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                cv.Skills = input.Split(',').Select(s => s.Trim()).ToList();
            }

            Console.Write($"GitHub Link ({cv.GitLink}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) cv.GitLink = input;

            Console.Write($"LinkedIn Link ({cv.LinkedIn}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) cv.LinkedIn = input;

            Database.Save();
            Database.Log($"{w.Username} CV-sini yeniledi (ID: {id})");
            Console.WriteLine("CV ugurla yenilendi!");
        }

        static void DeleteCV(Worker w)
        {
            ShowCVs(w);
            if (w.CVs.Count == 0) return;

            Console.Write("Silinecek CV ID daxil edin: ");
            int id = int.Parse(Console.ReadLine());

            var cv = w.CVs.FirstOrDefault(x => x.Id == id);
            if (cv == null) throw new AppException($"ID={id} olan CV tapilmadi.");
            w.CVs.Remove(cv);
            Database.Save();
            Database.Log($"{w.Username} CV-sini sildi (ID: {id})");
            Console.WriteLine("CV ugurla silindi!");
        }

        static void ViewVacancies(Worker w)
        {
            Console.Write("Vakansiya axtarisi (Vezife, Seher ve ya Skill daxil edin, hamisi ucun bos buraxin): ");
            string search = Console.ReadLine().ToLower();

            var allVacancies = Database.Employers.SelectMany(e => e.Vacancies);
            var results = string.IsNullOrWhiteSpace(search) ? allVacancies.ToList() : allVacancies.Where(v =>
                v.JobTitle.ToLower().Contains(search) || v.City.ToLower().Contains(search) ||
                v.RequiredSkills.Any(s => s.ToLower().Contains(search))).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("Uygun vakansiya tapilmadi.");
                return;
            }
            foreach (var v in results) v.Display();

            Console.Write("Muraciet etmek isteyirsiniz? (e/x): ");
            if (Console.ReadLine().Trim().ToLower() != "e")
                return;

            Console.Write("Elan ID daxil edin: ");
            int id = int.Parse(Console.ReadLine());

            var vacancy = results.FirstOrDefault(v => v.Id == id);
            if (vacancy == null) throw new AppException($"ID={id} olan elan tapilmadi.");
            if (!vacancy.IsActive) throw new AppException("Bu elanin muraciet muddeti bitib.");
            if (vacancy.Applicants.Contains(w.Username)) throw new AppException("Artiq bu elana muraciet etmisiniz.");

            if (w.CVs.Count == 0)
            {
                Console.WriteLine("Muraciet ucun evvelce CV yaratmalisiniz!");
                AddCV(w);
            }

            vacancy.Applicants.Add(w.Username);
            OnNewApplication?.Invoke(w.Username, vacancy.JobTitle);
            Database.Save();
            Console.WriteLine("Muracietiniz ugurla gonderildi!");
        }

        static void EmployerMenu(Employer e)
        {
            while (true)
            {
                Console.WriteLine($"\nIsegoturen Paneli: {e.Name} {e.Surname}");
                Console.WriteLine("1.Elan ver  2.Elanlarim  3.Elan Redakte et  4.Elan Sil  5.Namized Axtar  6.Bildirisler  0.Cixis");
                Console.Write("Seciminiz: ");

                if (!int.TryParse(Console.ReadLine(), out int ch))
                {
                    Console.WriteLine("Yanlış seçim.");
                    continue;
                }

                try
                {
                    switch (ch)
                    {
                        case 1:
                            PostVacancy(e);
                            break;
                        case 2:
                            ManageVacancies(e);
                            break;
                        case 3:
                            EditVacancy(e);
                            break;
                        case 4:
                            DeleteVacancy(e);
                            break;
                        case 5:
                            SearchCandidates();
                            break;
                        case 6:
                            ShowNotifications(e.Notifications);
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Yanlis secim.");
                            break;
                    }
                }
                catch (AppException ex)
                {
                    Console.WriteLine("Xeta: " + ex.Message);
                }
            }
        }

        static void PostVacancy(Employer e)
        {
            var v = new Vacancy();
            v.GenerateNewId();

            v.EmployerUsername = e.Username;
            Console.Write("Vezife : "); v.JobTitle = Console.ReadLine();
            Console.Write("Sirket Adi: "); v.CompanyName = Console.ReadLine();
            Console.Write("Seher: "); v.City = Console.ReadLine();
            Console.Write("Is Tipi (Full-time, Remote ve s): "); v.JobType = Console.ReadLine();
            Console.Write("Tecrube Seviyyesi: "); v.ExperienceLevel = Console.ReadLine();
            Console.Write("Min Maas: "); v.MinSalary = double.Parse(Console.ReadLine());
            Console.Write("Max Maas: "); v.MaxSalary = double.Parse(Console.ReadLine());
            Console.Write("Son muraciet tarixi : "); v.Deadline = DateTime.Parse(Console.ReadLine());
            Console.Write("Aciqlama: "); v.Description = Console.ReadLine();
            Console.Write("Teleb olunan bacariqlar: ");
            v.RequiredSkills = Console.ReadLine().Split(',').Select(s => s.Trim()).ToList();

            v.IsActive = true;

            e.Vacancies.Add(v);
            Database.Save();

            OnNewVacancy?.Invoke(e.Username, $"YENI VAKANSIYA: {v.CompanyName} sirketi \"{v.JobTitle}\" ucun elan yerlesdirdi!");
            Console.WriteLine("Elan ugurla yerlesdirildi!");
        }

        static void ManageVacancies(Employer e)
        {
            if (e.Vacancies.Count == 0)
            {
                Console.WriteLine("Elaniniz yoxdur.");
                return;
            }
            foreach (var v in e.Vacancies) v.Display();

            Console.Write("Muraciet eden namizedlere baxmaq isteyirsiniz? (e/x): ");
            if (Console.ReadLine().Trim().ToLower() != "e")
                return;

            Console.Write("Elan ID daxil edin: ");
            int id = int.Parse(Console.ReadLine());

            var vacancy = e.Vacancies.FirstOrDefault(v => v.Id == id);
            if (vacancy == null) throw new AppException($"ID={id} olan elan tapilmadi.");
            if (vacancy.Applicants.Count == 0)
            {
                Console.WriteLine("Bu elana hele ki muraciet yoxdur.");
                return;
            }

            foreach (var username in vacancy.Applicants)
            {
                var worker = Database.Workers.FirstOrDefault(i => i.Username == username);
                if (worker == null || worker.CVs.Count == 0)
                    continue;

                Console.WriteLine($"\nNamized: {worker.Name} {worker.Surname} ({worker.Username})");
                worker.CVs.Last().Display();

                Console.Write("Bu namizedi ise gebul etmek isteyirsiniz? (e/x): ");
                if (Console.ReadLine().Trim().ToLower() == "e")
                {
                    vacancy.SelectedWorker = worker.Username;
                    OnWorkerSelected?.Invoke(worker.Username, vacancy.JobTitle);
                    Database.Save();
                    Console.WriteLine("Namized ugurla secildi ve bildiris gonderildi!");
                    break;
                }
            }
        }

        static void EditVacancy(Employer e)
        {
            if (e.Vacancies.Count == 0)
            {
                Console.WriteLine("Elaniniz yoxdur.");
                return;
            }
            foreach (var v in e.Vacancies) v.Display();

            Console.Write("Redakte olunacaq elan ID daxil edin: ");
            int id = int.Parse(Console.ReadLine());

            var vacancy = e.Vacancies.FirstOrDefault(v => v.Id == id);
            if (vacancy == null) throw new AppException($"ID={id} olan elan tapilmadi.");

            Console.Write($"Vezife ({vacancy.JobTitle}), yenisi (Deyismemek ucun bos buraxin): ");
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) vacancy.JobTitle = input;

            Console.Write($"Sirket ({vacancy.CompanyName}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) vacancy.CompanyName = input;

            Console.Write($"Seher ({vacancy.City}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) vacancy.City = input;

            Console.Write($"Is tipi ({vacancy.JobType}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) vacancy.JobType = input;

            Console.Write($"Tecrube seviyyesi ({vacancy.ExperienceLevel}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) vacancy.ExperienceLevel = input;

            Console.Write($"Min maas ({vacancy.MinSalary}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) vacancy.MinSalary = double.Parse(input);

            Console.Write($"Max maas ({vacancy.MaxSalary}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) vacancy.MaxSalary = double.Parse(input);

            Console.Write($"Son muraciet tarixi ({vacancy.Deadline:dd.MM.yyyy}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) vacancy.Deadline = DateTime.Parse(input);

            Console.Write($"Aciqlama ({vacancy.Description}), yenisi (Deyismemek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) vacancy.Description = input;

            Console.Write("Bacariqlar (Yenilememek ucun bos buraxin): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                vacancy.RequiredSkills = input.Split(',').Select(s => s.Trim()).ToList();
            }

            Database.Save();
            Database.Log($"{e.Username} elanini yeniledi (ID: {id})");
            Console.WriteLine("Elan ugurla yenilendi!");
        }

        static void DeleteVacancy(Employer e)
        {
            if (e.Vacancies.Count == 0)
            {
                Console.WriteLine("Elaniniz yoxdur."); return;
            }
            foreach (var v in e.Vacancies) v.Display();

            Console.Write("Silinecek elan ID daxil edin: ");
            int id = int.Parse(Console.ReadLine());

            var vacancy = e.Vacancies.FirstOrDefault(v => v.Id == id);
            if (vacancy == null) throw new AppException($"ID={id} olan elan tapilmadi.");
            e.Vacancies.Remove(vacancy);

            Database.Save();
            Database.Log($"{e.Username} elanini sildi (ID: {id})");
            Console.WriteLine("Elan ugurla silindi!");
        }

        static void SearchCandidates()
        {
            Console.Write("Namized axtarisi (Ixtisas, Skill ve ya Seher daxil edin, hamisi ucun bos buraxin): ");
            string? search = Console.ReadLine()?.ToLower();

            var results = Database.Workers
                .SelectMany(i => i.CVs.Select(cv => new { Worker = i, CV = cv }))
                .Where(x => string.IsNullOrWhiteSpace(search) ||
                    x.CV.Skills.Any(s => s.ToLower().Contains(search)) ||
                    x.CV.Speciality.ToLower().Contains(search) ||
                    x.Worker.City.ToLower().Contains(search)).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("Axtarisa uygun namized tapilmadi.");
                return;
            }
            foreach (var x in results)
            {
                Console.WriteLine($"\nNamized: {x.Worker.Name} {x.Worker.Surname} ({x.Worker.Username}) | Seher: {x.Worker.City}");
                x.CV.Display();
            }
        }

        static void ShowNotifications(List<string> notifications)
        {
            if (notifications.Count == 0)
            {
                Console.WriteLine("Yeni bildirisiniz yoxdur.");
                return;
            }

            Console.WriteLine("\n=== BILDIRISLER ===");
            for (int i = 0; i < notifications.Count; i++) Console.WriteLine($"{i + 1}) {notifications[i]}");
            notifications.Clear();
            Database.Save();
        }
    }
}
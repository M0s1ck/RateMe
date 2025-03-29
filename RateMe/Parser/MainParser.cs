using RateMe.DataUtils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

using HtmlAgilityPack;

namespace RateMe.Parser
{
    internal class MainParser
    {
        internal List<Subject> Subjects { get; private set; } = [];

        private readonly SyllabusModel Syllabus;

        private string _curriculumNameShortened;
        private List<string> _subjectsUrls = [];
        private readonly Regex _curriculumRegex;
        private readonly HttpClient _httpClient = new HttpClient();


        private static readonly string BachUrl = @"https://www.hse.ru/education/bachelor";
        private static readonly string CurriculumCoursesPageUrlTemplate = @"https://www.hse.ru/ba/{0}/courses?course={1}.1.{1}.4&page={2}&year={3}";
        private static readonly int PageSubjectsCount = 20;

        private static readonly string CurriculumHtmlTemplate = @"<a class=""link"" href=""(https://www\.hse\.ru/ba/([a-z]+)/)"">{0}</a>";

        private static readonly string SubjectUrlXpathTemplate = "//html/body/div[1]/div[7]/div/div[2]/div/div/div[2]/div[{0}]";
        private static readonly string SubjectAssessmentNumerableXpathTemplate = "//html/body/div[1]/div[7]/div/div[2]/div/div/div/div[2]/div/div[3]/div/div[6]/div/ul/li[{0}]/div[1]";    
        private static readonly string SubjectFormulaNumerableXpathTemplate = "//html/body/div[1]/div[7]/div/div[2]/div/div/div/div[2]/div/div[3]/div/div[6]/div/ul/li[{0}]/div[2]";

        private static readonly string SubjectNameXpath = "//html/body/div[1]/div[7]/div/div[2]/div/div/div/div[1]/h1";
        private static readonly string SubjectCreditsXpath = "//html/body/div[1]/div[7]/div/div[2]/div/div/div/div[1]/div[2]/div[3]/div[1]";
        private static readonly string SubjectAssessmentXpath = "//html/body/div[1]/div[7]/div/div[2]/div/div/div/div[2]/div/div[2]/div/div[6]/div/ul/li/div[1]/span";
        private static readonly string SubjectFormulaXpath = "//html/body/div[1]/div[7]/div/div[2]/div/div/div/div[2]/div/div[2]/div/div[6]/div/ul/li/div[2]";
        private static readonly string SubjectModulesXpath = "//html/body/div[1]/div[7]/div/div[2]/div/div/div/div[2]/div/div[1]/div[2]/div[2]/dl/dd";
                                                          
        private static readonly int CurrentStudyYear = GetCurrentStudyYear();

        private static readonly Regex[] ModulesRegexes = [new(@"(\d)\s*,\s*(\d)"), new(@"(\d)\s*-\s*(\d)"), new(@"\d")];


        public MainParser(SyllabusModel syllabus)
        {
            Syllabus = syllabus;
            string curriculumReHtmlTemp = string.Format(CurriculumHtmlTemplate, syllabus.Curriculum);
            _curriculumRegex = new(curriculumReHtmlTemp);
        }


        internal async Task GetCurriculumAsync()
        {
            string bachPageContent = await _httpClient.GetStringAsync(BachUrl);

            Match currMatch = _curriculumRegex.Match(bachPageContent);

            if (!currMatch.Success)
            {
                MessageBox.Show($"Error: curriculum {Syllabus.Curriculum} was not found");
                return;
            }

            _curriculumNameShortened = currMatch.Groups[2].Value;
        }


        internal async Task GetSubjectsUrlsAltAsync()
        {
            List<string> subjectsUrls = [];
            int pageNumber = 1;
            HtmlNode? subjNode = null;
            bool quit = false;

            do
            {
                string curriculumCoursesPageUrl = string.Format(CurriculumCoursesPageUrlTemplate, _curriculumNameShortened, Syllabus.Course, pageNumber, CurrentStudyYear);

                using HttpResponseMessage response = await _httpClient.GetAsync(curriculumCoursesPageUrl);
                string pageContent = await response.Content.ReadAsStringAsync();

                HtmlDocument curriculumCoursesDoc = new HtmlDocument();
                curriculumCoursesDoc.LoadHtml(pageContent);              

                for (int i = 0; i < PageSubjectsCount; i++)
                {
                    string subjXpath = string.Format(SubjectUrlXpathTemplate, i + 2);
                    subjNode = curriculumCoursesDoc.DocumentNode.SelectSingleNode(subjXpath);

                    if (subjNode == null)
                    {
                        break;
                    }

                    HtmlAttribute? urlAttr = subjNode.Attributes["data-href"];

                    if (urlAttr == null)
                    {
                        quit = true;
                        break;
                    }

                    subjectsUrls.Add(urlAttr.Value);                  
                }    

                pageNumber++;
            }
            while (subjNode != null && !quit);

            _subjectsUrls = subjectsUrls;
        }


        internal async Task<List<Subject>> GetSubjectsDataAsync()
        {
            List<Subject> subjects = [];

            foreach(string subjectUrl in _subjectsUrls)
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(subjectUrl);
                
                if (response == null || !response.IsSuccessStatusCode)
                {
                    break;
                }

                string pageContent = await response.Content.ReadAsStringAsync();
                HtmlDocument subjDoc = new HtmlDocument();
                subjDoc.LoadHtml(pageContent);
                HtmlNode rootDocNode = subjDoc.DocumentNode;

                HtmlNode? subjNameNode = rootDocNode.SelectSingleNode(SubjectNameXpath);
                string subjName = subjNameNode?.InnerText ?? "SubjectNameNotFound";

                HtmlNode? subjCreditsNode = rootDocNode.SelectSingleNode(SubjectCreditsXpath);
                string subjCreditsText = subjCreditsNode?.InnerText ?? string.Empty;
                int credits = int.TryParse(subjCreditsText, out credits) ? credits : -1;

                Dictionary<string, string> assesementFormulas = GetAssesmentFormulas(rootDocNode);

                int[] modules = GetSubjectsModules(rootDocNode);

                Subject sub = new Subject(subjName, credits, modules, assesementFormulas);
                subjects.Add(sub);
            }

            Subjects = subjects;

            return Subjects;
        }


        private static Dictionary<string, string> GetAssesmentFormulas(HtmlNode rootDocNode)
        {
            Dictionary<string, string> assesementFormulas = [];
            HtmlNode? subjFormulaNode;
            HtmlNode? subjAssNode;
            int cnt = 1;

            do
            {
                string assXpath = string.Format(SubjectAssessmentNumerableXpathTemplate, cnt);
                subjAssNode = rootDocNode.SelectSingleNode(assXpath);
                string ass = subjAssNode?.InnerText ?? "AssessmentPeriodNotFound";

                string formulaXpath = string.Format(SubjectFormulaNumerableXpathTemplate, cnt);
                subjFormulaNode = rootDocNode.SelectSingleNode(formulaXpath);
                string formulaText = subjFormulaNode?.InnerText ?? "FormulaNotFound";

                if (ass != "AssessmentPeriodNotFound" || formulaText != "FormulaNotFound")
                {
                    assesementFormulas[ass] = formulaText;
                }

                cnt++;
            }
            while (subjFormulaNode != null || subjAssNode != null);

            if (assesementFormulas.Count > 0)
            {
                return assesementFormulas;
            }

            // if formulas are not listed as a "list"
            subjAssNode = rootDocNode.SelectSingleNode(SubjectAssessmentXpath);
            string asse = subjAssNode?.InnerText ?? "AssessmentPeriodNotFound";

            subjFormulaNode = rootDocNode.SelectSingleNode(SubjectFormulaXpath);
            string formText = subjFormulaNode?.InnerText ?? "FormulaNotFound";

            if (asse != "AssessmentPeriodNotFound" || formText != "FormulaNotFound")
            {
                assesementFormulas[asse] = formText;
            }

            return assesementFormulas;
        }
        

        private static int[] GetSubjectsModules(HtmlNode rootDocNode)
        {
            HtmlNode? modulesNode = rootDocNode.SelectSingleNode(SubjectModulesXpath);
            string modsFullText = modulesNode?.InnerText ?? "SubjectModulesNotFound";

            int commaInd = modsFullText.IndexOf(',');
            int wordInd = modsFullText.IndexOf('м');

            string modsText = modsFullText.Substring(commaInd + 1, wordInd - commaInd).Trim();

            int[] mods = [];

            if (ModulesRegexes[0].IsMatch(modsText))
            {
                Match match = ModulesRegexes[0].Match(modsText);
                mods = [ int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value) ];
            }
            else if (ModulesRegexes[1].IsMatch(modsText))
            {
                Match match = ModulesRegexes[1].Match(modsText);
                int mod1 = int.Parse(match.Groups[1].Value);
                int mod2 = int.Parse(match.Groups[2].Value);
                mods = Enumerable.Range(mod1, mod2-mod1 + 1).ToArray();
            }
            else if (ModulesRegexes[2].IsMatch(modsText))
            {
                Match match = ModulesRegexes[2].Match(modsText);
                mods = [ int.Parse(match.Groups[0].Value) ];
            }
            
            return mods;
        }


        private static int GetCurrentStudyYear()
        {
            DateTime now = DateTime.Now;
            return now.Month >= 8 ? now.Year : now.Year - 1;
        }



    }
}

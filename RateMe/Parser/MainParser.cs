using RateMe.DataUtils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace RateMe.Parser
{
    internal class MainParser
    {
        private readonly SyllabusModel Syllabus;

        private readonly Regex _curriculumRegex;
        private string _curriculumNameShortened;
        private List<string> _subjectsUrls;

        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly string BachUrl = @"https://www.hse.ru/education/bachelor";
        private static readonly string CurriculumCoursesPageUrlTemplate = @"https://www.hse.ru/ba/{0}/courses?course={1}.1.{1}.4&page={2}&year={3}";

        private static readonly string CurriculumHtmlTemplate = @"<a class=""link"" href=""(https://www\.hse\.ru/ba/([a-z]+)/)"">{0}</a>";
        private static readonly string SubjectHtmlTemplate = @"<a class=""link link_dark"" href=""(https://www\.hse\.ru/ba/{0}/courses/(\d+)\.html)"">([^<]+)</a>";

        private static readonly Regex SubjectNameRe = new(@"<h1 class=""b-program__header-title"">([^<]+)</h1>");

        private static readonly int CurrentStudyYear = GetCurrentStudyYear();


        public MainParser(SyllabusModel syllabus)
        {
            Syllabus = syllabus;
            string curriculumReHtmlTemp = string.Format(CurriculumHtmlTemplate, syllabus.Curriculum);
            _curriculumRegex = new(curriculumReHtmlTemp);
        }


        internal async Task GetCurriculumAsync()
        {
            string bachPageContent = await HttpClient.GetStringAsync(BachUrl);

            Match currMatch = _curriculumRegex.Match(bachPageContent);

            if (!currMatch.Success)
            {
                MessageBox.Show($"Error: curriculum {Syllabus.Curriculum} was not found");
                return;
            }

            _curriculumNameShortened = currMatch.Groups[2].Value;
        }


        internal async Task GetSubjectsUrlsAsync()
        {
            int pageNumber = 1;
            string subjectPattern = string.Format(SubjectHtmlTemplate, _curriculumNameShortened);
            List<string> subjectsUrls = [];

            while (true)
            {
                string curriculumCoursesPageUrl = string.Format(CurriculumCoursesPageUrlTemplate, _curriculumNameShortened, Syllabus.Course, pageNumber, CurrentStudyYear);

                using HttpResponseMessage response = await HttpClient.GetAsync(curriculumCoursesPageUrl);

                if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    break;
                }

                string curriculumCoursesPageContent = await response.Content.ReadAsStringAsync();

                MatchCollection subjectsMatches = Regex.Matches(curriculumCoursesPageContent, subjectPattern);

                if (subjectsMatches.Count == 0)
                {
                    break;
                }

                foreach (Match subjectMatch in subjectsMatches)
                {
                    subjectsUrls.Add(subjectMatch.Groups[1].Value);
                }

                pageNumber++;
            }

            _subjectsUrls = subjectsUrls;
        }


        internal async Task GetSubjectsDataAsync()
        {
            foreach(string subjectUrl in _subjectsUrls)
            {
                string pageContent = await HttpClient.GetStringAsync(BachUrl);

                string subjectName = SubjectNameRe.Match(pageContent).Value;

            }
        }


        private static int GetCurrentStudyYear()
        {
            DateTime now = DateTime.Now;
            return now.Month >= 8 ? now.Year : now.Year - 1;
        }

    }
}

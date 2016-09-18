using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace GetGameRankingsData
{
    public partial class Form1 : Form
    {
        private DataTable table;
        private HtmlWeb web = new HtmlWeb();
        public Form1()
        {
            InitializeComponent();
            InitTable();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            int i = 0;
            var rankings = await GameRankingsFromPage(i);
            while (rankings.Count > 0)
            {
                foreach (var ranking in rankings) table.Rows.Add(ranking.Name, ranking.Score);
                rankings = await GameRankingsFromPage(++i);
            }
        }

        private async Task<List<Game>> GameRankingsFromPage(int pageNum)
        {

            string url;
            if (pageNum == 0) url = "http://www.gamerankings.com/browse.html";
            else url = "http://www.gamerankings.com/browse.html?page=" + pageNum.ToString();


            var doc = await Task.Factory.StartNew(() => web.Load(url));
            var nodesNames = doc.DocumentNode.SelectNodes("//*[@id=\"main_col\"]/div//div//table//tr//td//a");
            var gameNames = nodesNames.Select(node => node.InnerText);

            var nodesPercentages = doc.DocumentNode.SelectNodes("//*[@id=\"main_col\"]/div//div//table//tr//td//span/b");
            var gamePercentages = nodesPercentages.Select(node => node.InnerText);

            // If either of these lists are empty then  we return an empty list.
            if (nodesNames == null || nodesPercentages == null) return new List<Game>();

            return gameNames.Zip(gamePercentages, (name, score) => new Game() { Name = name, Score = score }).ToList();
        }

        private void InitTable()
        {
            table = new DataTable("GameRankingsDataTable");
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Score", typeof(string));
            gameRankingsDataView.DataSource = table;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ConsoleApplication.nsDataObjects.nsData.nsBusinessObjects;
using ConsoleApplication.nsDataObjects.nsResults;
using ConsoleApplication.nsSolver;

namespace ConsoleApplication
{
    /// <summary>
    /// Moved chart creation and migrated result output to allow everything to be seen in one screen.
    /// Currently you can choose how many results you wish to view and whether it's the top or bottom ones your interested in. 
    /// </summary>
    public partial class GraphViewer : Form
    {
       //Not a paragon of architectural beauty, separation or anything like it. Too much coupling going on!
       //If we don't have a solver, get one, otherwise reuse the current one.

        private Task<ISolver> _solver;
        private Task<ISolver> Solver { get { return _solver ?? (_solver = Task.Factory.StartNew(()=> nsFactory.Factory.GetSolver())); } }

        public GraphViewer(Stopwatch stopwatch)
        {
            InitializeComponent();
            OutputBox.Text = "Please wait whilst the results are built";
           

            Solver.ContinueWith(a =>
                {
                    OutputBox.Text = "";
                    foreach (Result result in a.Result.Solve())
                    {
                        OutputBox.Text += string.Format("{0} {1}", result, Environment.NewLine);
                    }
                    stopwatch.Stop();
                    TimeSpan elapsed = stopwatch.Elapsed;
                    OutputBox.Text += String.Format("Time Elapsed: {0:00}:{1:00}", elapsed.Seconds, elapsed.Milliseconds);
                    foreach (Treatment treatment in a.Result.Treatments) DrugPicker.Items.Add(treatment);
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

      private void Graphs_Load(object sender, EventArgs e)
        {
            DisplayActualCostResults(Order.Top);
        }

        /// <summary>
        /// Get how many results you want, make sure it's a number! Should be a numerical control ideally.
        /// </summary>
       
        private int ResultCount()
        {
                int count;
                bool testInput = int.TryParse(NumberOfResultsBox.Text, out count);
                return testInput ? int.Parse(NumberOfResultsBox.Text) : 5;
          
        }

        private void NumberOfResultsBox_Click(object sender, EventArgs e)
        {
          ResultCount();
        }

        private void DisplayActualCostResults( Order order)
        {
            Solver.ContinueWith(a => MakeGraph(a.Result.Results.OrderBy(x => x.Value).ToDictionary(t => t.Key, t => t.Value),
                                                 String.Format("{0} {1} PostCodes Actual Spend", order, ResultCount()), order), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void MakeGraph(Dictionary<string, decimal> results, String title, Order order)
        {
           
            chart.Refresh();
            ChartArea chartArea = new ChartArea
                {
                    AxisX = {Interval = 1, LabelStyle = {Angle = -90}, IsLabelAutoFit = false}
                };
            
                var _results = order == Order.Top ? results.Reverse().Take(ResultCount()) : results.Take(ResultCount());
                chart.ChartAreas.Clear();

                this.chart.ChartAreas.Add(chartArea);

                chart.Series.Clear();
                chart.Titles.Clear();

                chart.Titles.Add(title);
                chart.ChartAreas[0].RecalculateAxesScale();
                chart.Series.Add("Data");
                foreach (var result in _results)
                    {
                        chart.Series[0].Points.AddXY(result.Key, result.Value);
                    }
            
        }
   

        // Very simple enum to allow the order to be included in graph title as well.
    internal enum Order
        {
            Top  = 0 ,
            Bottom  = 1 
        }
   
        private void DisplayActualCostResultsBottom_Click(object sender, EventArgs e)
        {
            DisplayActualCostResults( Order.Bottom);
        }

        private void DisplayActualCostResultsTop_Click(object sender, EventArgs e)
        {
            DisplayActualCostResults(Order.Top);
        }

        /// <summary>
        /// Return a graph based on the treatment you wish to look at.
        /// This mixes basic UI and data querying - not a layered approach.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
    
 
        private void DrugPicker_Click(object sender, EventArgs e)
        {
            var item = DrugPicker.SelectedItem as Treatment;
            if (item != null)
            {
                Treatment selectedTreatment = item;
 
             
                   
                //Lets not lock up the form!
                Solver.ContinueWith(a =>
                    {
                        Dictionary<string, decimal> data = a.Result.GetDataPerBnfCode(ResultCount())
                                                            .Where(d => d.Key == selectedTreatment.BNFCode)
                                                            .ContainsKeyExt();
                        MakeGraph(data,String.Format("{0} {1} PostCodes Actual Spend on {0}", selectedTreatment.Name,
                                                ResultCount()),
                                  Order.Top);

                    }, TaskScheduler.FromCurrentSynchronizationContext());

            }
        }
        
    }

    public static class Extensions
    {
        /// <summary>
        /// Minor overkill - Extension to handle the duplicate key entry check used when processing the grouped by treatment type results. 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Dictionary<String, decimal> ContainsKeyExt(this IEnumerable<KeyValuePair<String, List<GpPrescriptionData>>> values)
        {
           
            var  temp = new Dictionary<string, decimal>();
            if (values != null)
                foreach (var kvp in values)
                {
               
                    foreach (var item in kvp.Value)
                    {
                        string practiceName = Solver.GetPracticeViaCode(item.PRACTICE);
                        if (!temp.ContainsKey(practiceName)) temp.Add(practiceName, item.ACTCOST);
                    }
                }
            return temp;
        }
    }
      
    }


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace OperatorReport.Models
{
    public class OperatorReportViewModel
    {
        public int ID { get; set; }
        public string Name { get; set;}
        public int ProactiveSent { get; set; }
        public int ProactiveAnswered { get; set; }
        public int ProactiveResponseRate { get; set; }
        public int ReactiveReceived { get; set; }
        public int ReactiveAnswered { get; set; }
        public int ReactiveResponseRate { get; set; }
        public int TotalChatLengthSeconds { get; set; }
        public int AverageChatLengthSeconds { get; set; }
    }

    public class OperatorReportItems : Collection<OperatorReportViewModel>
    {
        //private ICollection<OperatorReportViewModel> OperatorProductivity = new collection
    }
}
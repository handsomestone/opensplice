namespace test.sacs
{
    /// <date>Jun 20, 2005</date>
    public class Waitset6 : Test.Framework.TestCase
    {
        public Waitset6()
            : base("sacs_waitset_tc6", "waitset", "waitset", "test datareader status condition in waitset"
                , "test datawriter status condition in waitset", null)
        {
            this.AddPreItem(new test.sacs.WaitsetInit());
            this.AddPostItem(new test.sacs.WaitsetDeinit());
        }

        public override Test.Framework.TestResult Run()
        {
            mod.tstDataWriter writer;
            mod.tstDataReader reader;
            mod.tstDataReader reader2;
            DDS.IStatusCondition condition;
            DDS.WaitSet waitset;
            DDS.ISubscriber subscriber;
            DDS.ICondition[] conditionHolder;
            Test.Framework.TestResult result;
            DDS.DataReaderQos drQos;
            DDS.ITopic topic;
            DDS.ReturnCode rc;
            string expResult = "StatusCondition test succeeded.";
            result = new Test.Framework.TestResult(expResult, string.Empty, Test.Framework.TestVerdict
                .Pass, Test.Framework.TestVerdict.Fail);
            writer = (mod.tstDataWriter)this.ResolveObject("datawriter");
            reader = (mod.tstDataReader)this.ResolveObject("datareader");
            subscriber = (DDS.ISubscriber)this.ResolveObject("subscriber");
            topic = (DDS.ITopic)this.ResolveObject("topic");
            drQos = (DDS.DataReaderQos)this.ResolveObject("datareaderQos");
            condition = writer.StatusCondition;
            if (condition == null)
            {
                result.Result = "Could not resolve reader condition.";
                return result;
            }
            waitset = new DDS.WaitSet();
            rc = waitset.AttachCondition(condition);
            if (rc != DDS.ReturnCode.Ok)
            {
                result.Result = "Could not attach condition.";
                return result;
            }
            reader2 = (mod.tstDataReader)subscriber.CreateDataReader(topic, drQos);//, null, 0);
            if (reader2 == null)
            {
                result.Result = "Could not create datareader.";
                return result;
            }
            conditionHolder = new DDS.ICondition[0];
            rc = waitset.Wait(ref conditionHolder, new DDS.Duration(3, 0));
            if (rc != DDS.ReturnCode.Timeout)
            {
                result.Result = "WaitSet.Wait failed.";
                return result;
            }
            if (conditionHolder.Length != 0)
            {
                this.PrintStatusses(writer);
                result.Result = "WaitSet.Wait returned conditions where it shouldn't (2).";
                return result;
            }
            rc = subscriber.DeleteDataReader(reader);
            if (rc != DDS.ReturnCode.Ok)
            {
                result.Result = "delete_datareader failed.";
                return result;
            }
            rc = waitset.Wait(ref conditionHolder, new DDS.Duration(3, 0));
            if (rc != DDS.ReturnCode.Timeout)
            {
                result.Result = "WaitSet.Wait failed(3).";
                return result;
            }
            if (conditionHolder.Length > 0)
            {
                PrintStatusses(writer);
                result.Result = "WaitSet.Wait returned conditions where it shouldn't.(3)";
                return result;
            }
            result.Result = expResult;
            result.Verdict = Test.Framework.TestVerdict.Pass;
            return result;
        }

        private void PrintStatusses(DDS.IDataWriter writer)
        {
            DDS.ReturnCode rc;
			DDS.OfferedDeadlineMissedStatus odsHolder = null;
            rc = writer.GetOfferedDeadlineMissedStatus(ref odsHolder);
            DDS.OfferedDeadlineMissedStatus ods = odsHolder;
            if (rc != DDS.ReturnCode.Ok)
            {
                System.Console.Error.WriteLine("Unable to resolve status!");
                return;
            }
			DDS.OfferedIncompatibleQosStatus oisHolder = null;
            rc = writer.GetOfferedIncompatibleQosStatus(ref oisHolder);
            DDS.OfferedIncompatibleQosStatus ois = oisHolder;
            if (rc != DDS.ReturnCode.Ok)
            {
                System.Console.Error.WriteLine("Unable to resolve status!");
                return;
            }
			DDS.LivelinessLostStatus llsHolder = null;
            rc = writer.GetLivelinessLostStatus(ref llsHolder);
            DDS.LivelinessLostStatus lls = llsHolder;
            if (rc != DDS.ReturnCode.Ok)
            {
                System.Console.Error.WriteLine("Unable to resolve status!");
                return;
            }
			DDS.PublicationMatchedStatus pmsHolder = null;
            rc = writer.GetPublicationMatchedStatus(ref pmsHolder);
            DDS.PublicationMatchedStatus pms = pmsHolder;
            if (rc != DDS.ReturnCode.Ok)
            {
                System.Console.Error.WriteLine("Unable to resolve status!");
                return;
            }
            System.Console.Out.WriteLine("offered_deadline_missed.TotalCount           : " +
                 ods.TotalCount);
            System.Console.Out.WriteLine("offered_deadline_missed.TotalCountChange    : " +
                 ods.TotalCountChange);
            System.Console.Out.WriteLine("offered_deadline_missed.LastInstanceHandle  : " +
                 ods.LastInstanceHandle);
            System.Console.Out.WriteLine("offered_incompatible_qos.TotalCount          : " +
                 ois.TotalCount);
            System.Console.Out.WriteLine("offered_incompatible_qos.TotalCountChange   : " +
                 ois.TotalCountChange);
            System.Console.Out.WriteLine("offered_incompatible_qos.LastPolicyId       : " +
                 ois.LastPolicyId);
            System.Console.Out.WriteLine("liveliness_lost.TotalCount                   : " +
                 lls.TotalCount);
            System.Console.Out.WriteLine("liveliness_lost.TotalCountChange            : " +
                 lls.TotalCountChange);
            System.Console.Out.WriteLine("publication_matched.TotalCount               : " +
                 pms.TotalCount);
            System.Console.Out.WriteLine("publication_matched.TotalCountChange        : " +
                 pms.TotalCountChange);
            System.Console.Out.WriteLine("publication_matched.LastSubscriptionHandle  : " +
                 pms.LastSubscriptionHandle);
        }
    }
}

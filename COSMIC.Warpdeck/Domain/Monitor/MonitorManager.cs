using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Monitor.Rules;
using COSMIC.Warpdeck.UseCase.Device;

namespace COSMIC.Warpdeck.Domain.Monitor
{
    public class MonitorManager
    {
        private List<IMonitor> Monitors { get; } = new();
        public Dictionary<string, MonitorRule> MonitorRules { get; } = new();
        public Dictionary<string, MonitorRuleModel> MonitorRuleModels { get; } = new();
        private bool _isListening = true;


        public void AddMonitor(IMonitor monitor)
        {
            monitor.OnMonitorChange += MonitorOnOnMonitorChange;
            Monitors.Add(monitor);
        }

        public void StopListening()
        {
            _isListening = false;
        }

    

        private void MonitorOnOnMonitorChange(IMonitor sender, MonitorChangeEventArgs args)
        {
            if (!_isListening)
                return;

            var metActions = new List<IMonitorRuleAction>();
            var unmetActions = new List<IMonitorRuleAction>();

            foreach (var monitorRuleKvp in MonitorRules)
            {
                bool result = monitorRuleKvp.Value.Criteria.IsMetBy(args);
                foreach (var x in monitorRuleKvp.Value.Actions)
                {
                    if (result)
                        metActions.Add(x);
                    else
                        unmetActions.Add(x);
                }
            }
            
            unmetActions.ForEach(x => x.ActionWhenFalse());
            metActions.ForEach(x => x.ActionWhenTrue());
            // This sucks
            WarpDeckApp.Container.Resolve<DeviceManager>().RedrawDevices();
                
        }

        
        
        public void AddMonitorRule(MonitorRuleModel ruleModel)
        {
            MonitorRule newRule = new MonitorRule();

            var criteriaParams =
                ruleModel.Criteria.Parameters.Select(x => new NamedParameter(x.Key, x.Value)).ToArray();
            newRule.Criteria =
                WarpDeckApp.Container.ResolveNamed<MonitorCondition>(ruleModel.Criteria.CriteriaType,
                    criteriaParams);

            foreach (MonitorRuleActionModel modelAction in ruleModel.Actions)
            {
                var actionParams = modelAction.Parameters.Select(x => new NamedParameter(x.Key, x.Value));
                IMonitorRuleAction action =
                    WarpDeckApp.Container.ResolveNamed<IMonitorRuleAction>(modelAction.ActionType, actionParams);
                newRule.Actions.Add(action);
            }
            Guid monitorId = Guid.NewGuid();
            MonitorRuleModels.Add(monitorId.ToString(), ruleModel);
            MonitorRules.Add(monitorId.ToString(), newRule);
        }
    }
}

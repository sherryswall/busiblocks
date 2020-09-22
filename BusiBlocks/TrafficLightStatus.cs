using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusiBlocks.Audit;
using BusiBlocks.Versioning;

namespace BusiBlocks
{
    public class TrafficLightStatus
    {
        public bool RequiresAck { get; set; }
        public bool Viewed { get; set; }
        public bool Acknowledged { get; set; }
    }
    public static class TrafficLightHelper
    {
        public static TrafficLightStatus GetTrafficLight(string userName, string itemId, bool requiresAck)
        {
            TrafficLightStatus tlStatus = new TrafficLightStatus();
            string groupId = VersionManager.GetVersionByItemId(itemId).GroupId;

            IList<VersionItem> publishedVersions = VersionManager.GetPublishedVersions(groupId);

            int currentVersPubMajPart = Int32.Parse(publishedVersions[0].VersionNumber.Split('.')[0]);
            //for a minor -> minor change

            if (publishedVersions[0].EditSeverity.Equals("minor"))
            {
                foreach (VersionItem version in publishedVersions)
                {
                    int temp = Int32.Parse(version.VersionNumber.Split('.')[0]);
                    if (temp == currentVersPubMajPart)
                    {
                        //reset traffic light if ack required else check audit table(reset if no record exists)
                        //the scenario for traffic lights when its a major publish and acknowledgement is not required(traffic light stays green) is not covered here since we are not capturing that in Audit table.            
                        if (!requiresAck)
                        {
                            if (AuditManager.GetAuditItems(userName, version.ItemId, AuditRecord.AuditAction.Viewed).Count > 0)
                            {
                                tlStatus.Viewed = true;
                                break;
                            }
                            if (AuditManager.GetAuditItems(userName, version.ItemId, AuditRecord.AuditAction.Acknowledged).Count > 0)
                            {
                                tlStatus.Viewed = true;
                                break;
                            }
                        }
                        else
                        {
                            if (AuditManager.GetAuditItems(userName, version.ItemId, AuditRecord.AuditAction.Acknowledged).Count > 0)
                            {
                                tlStatus.Acknowledged = true;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (!requiresAck)
                {
                    if (AuditManager.GetAuditItems(userName, itemId, AuditRecord.AuditAction.Viewed).Count > 0)
                    {

                        tlStatus.Viewed = true;
                    }
                }
                else
                {                   
                    if (AuditManager.GetAuditItems(userName, itemId, AuditRecord.AuditAction.Acknowledged).Count > 0)
                    {
                        tlStatus.Acknowledged = true;
                    }
                }
            }
            tlStatus.RequiresAck = requiresAck;
            return tlStatus;
        }
    }
}

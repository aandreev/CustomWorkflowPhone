using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace SDAActivityLibrary
{
    public class UpdatePhoneRegarding : CodeActivity
{ 
        [Output("error code")]
        [Default("undef")]
        public OutArgument<string> Success { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();

            if (workflowContext.InputParameters.Contains("Target") &&
            workflowContext.InputParameters["Target"] is Entity)
{
                Entity target = (Entity)workflowContext.InputParameters["Target"];
                EntityReference entityRef = new EntityReference();
                entityRef.Id = target.Id;
                entityRef.LogicalName = target.LogicalName;
                // Obtain the organization service reference.
                IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

                // Use the context service to create an instance of IOrganizationService.
                IOrganizationService _orgService = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);
                if (target.LogicalName == "phonecall")
                {
                    Entity phonecall = _orgService.Retrieve("phonecall", target.Id, new ColumnSet("to"));

                    EntityReference partyId = null;
                    EntityCollection to = phonecall.GetAttributeValue<EntityCollection>("to");
                    if (to != null)
                    {
                        to.Entities.ToList().ForEach(party =>
                        {
                            bool isDeleted = party.GetAttributeValue<bool>("ispartydeleted");
                            if (!isDeleted)
                            {
                                partyId = party.GetAttributeValue<EntityReference>("partyid");
                            }
                        });
                    }
                    if (partyId != null)
                    {
                        phonecall["regardingobjectid"] = partyId;
                        _orgService.Update(phonecall);
                        Success.Set(context, "success");
                    }
                    else
                    {
                        Success.Set(context, "fail");
                    }
              
                }
                else
                {
                    Success.Set(context, "fail");
                }
    

                    
            }

        }
    }
}

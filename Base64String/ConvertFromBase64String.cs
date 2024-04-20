// Copyright (c) 2024 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Management.Automation;

namespace RhubarbGeekNz.Base64String
{
    [Cmdlet(VerbsData.ConvertFrom, "Base64String")]
    [OutputType(typeof(byte[]))]
    sealed public class ConvertFromBase64String : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Base64 Data")]
        [AllowNull()]
        [AllowEmptyString()]
        public string InputString;

        protected override void ProcessRecord()
        {
            try
            {
                WriteObject(InputString == null ? null : System.Convert.FromBase64String(InputString));
            }
            catch (FormatException ex)
            {
                WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidData, null));
            }
        }
    }
}

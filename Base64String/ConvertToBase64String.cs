// Copyright (c) 2024 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Management.Automation;

namespace RhubarbGeekNz.Base64String
{
    [Cmdlet(VerbsData.ConvertTo, "Base64String")]
    [OutputType(typeof(string))]
    sealed public class ConvertToBase64String : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Binary Data")]
        [AllowNull()]
        [AllowEmptyCollection()]
        public byte[] Value;

        protected override void ProcessRecord()
        {
            WriteObject(Value == null ? null : System.Convert.ToBase64String(Value));
        }
    }
}

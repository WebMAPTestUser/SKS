using UpgradeHelpers.VB6.DB.ADO;
using System;
using System.Data.Common;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal static class modConnection
	{


		private static DbConnection _CurrentConnection = null;
		internal static DbConnection CurrentConnection
		{
			get
			{
				if (_CurrentConnection == null)
				{
					_CurrentConnection = UpgradeHelpers.VB6.DB.AdoFactoryManager.GetFactory().CreateConnection();
				}
				return _CurrentConnection;
			}
			set
			{
				_CurrentConnection = value;
			}
		}

		private static ADORecordSetHelper _rs = null;
		internal static ADORecordSetHelper rs
		{
			get
			{
				if (_rs == null)
				{
					_rs = new ADORecordSetHelper("");
				}
				return _rs;
			}
			set
			{
				_rs = value;
			}
		}

		private static ADORecordSetHelper _rs2 = null;
		internal static ADORecordSetHelper rs2
		{
			get
			{
				if (_rs2 == null)
				{
					_rs2 = new ADORecordSetHelper("");
				}
				return _rs2;
			}
			set
			{
				_rs2 = value;
			}
		}


		internal static void OpenConnection()
		{
			CurrentConnection = UpgradeHelpers.VB6.DB.AdoFactoryManager.GetFactory().CreateConnection();
			//UPGRADE_TODO: (7010) The connection string must be verified to fullfill the .NET data provider connection string requirements. More Information: http://www.vbtonet.com/ewis/ewi7010.aspx
			CurrentConnection.ConnectionString = modMain.ConnectionString;
			CurrentConnection.Open();
		}

		internal static void ExecuteSql(string Statement)
		{
			rs = new ADORecordSetHelper("");
			rs.Open(Statement, CurrentConnection, LockTypeEnum.adLockPessimistic);
		}

		internal static void ExecuteSql2(string Statement)
		{
			rs2 = new ADORecordSetHelper("");
			rs2.Open(Statement, CurrentConnection, LockTypeEnum.adLockPessimistic);
		}
	}
}
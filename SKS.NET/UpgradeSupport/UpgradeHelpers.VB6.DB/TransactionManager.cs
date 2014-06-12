using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Threading;

namespace UpgradeHelpers.VB6.DB
{
    /// <summary>
    /// Transaction Manager Class, used to control Database transactions
    /// </summary>
    public class TransactionManager
    {
        private static Dictionary<DbConnection, List<DbTransaction>> _transactions = new Dictionary<DbConnection, List<DbTransaction>>();

        /// <summary>
        /// Returns the Transaction object associated to a connection.
        /// </summary>
		/// <param name="dbh">The DAODatabaseHelper to get the transaction from.</param>
		/// <returns>The transaction associated with the parameter.</returns>
		public static DbTransaction GetTransaction(DAO.DAODatabaseHelper dbh)
		{
			return GetTransaction(dbh.Connection);
		}

        /// <summary>
        /// Returns the Transaction object associated to a connection.
        /// </summary>
        /// <param name="conn">The connection to get the transaction from.</param>
        /// <returns>The transaction associated with the parameter.</returns>
        public static DbTransaction GetTransaction(DbConnection conn)
        {
            DbTransaction t = null;
            lock (_transactions)
            {
                if (_transactions.ContainsKey(conn))
                {
                    t = GetCurrentTransaction(conn);
                }
            }
            return t;
        }
        /// <summary>
        /// Gets the current transaction for the specified connection.
        /// </summary>
        /// <param name="conn">The connection to get the transaction from.</param>
        /// <returns>The transaction associated with the parameter.</returns>
        private static DbTransaction GetCurrentTransaction(DbConnection conn)
        {
            List<DbTransaction> transactions = _transactions[conn];
            return transactions[transactions.Count - 1];
        }

        /// <summary>
        /// Commits the transaction associated to the specified connection. Once the Commit is performed the transaction is DeEnlisted.
        /// </summary>
        /// <param name="conn"></param>
        public static void Commit(DbConnection conn)
        {
            DbTransaction t = null;
            lock (_transactions)
            {
                if (_transactions.ContainsKey(conn))
                {
                    t = GetCurrentTransaction(conn);
                    t.Commit();
                    DeEnlist(conn, t);
                }
            }
        }


        /// <summary>
        /// Rollbacks the transaction associated to the specified connection. Once the Rollback is performed the connection is DeEnlisted.
        /// </summary>
        /// <param name="conn">The connection to get the transaction from.</param>
        public static void Rollback(DbConnection conn)
        {
            DbTransaction t = null;
            lock (_transactions)
            {
                if (_transactions.ContainsKey(conn))
                {
                    t = GetCurrentTransaction(conn);
                    t.Rollback();
                    DeEnlist(conn, t);
                }
            }
        }

        /// <summary>
        /// Enlists a Transaction in the transaction manager.
        /// </summary>
        /// <param name="conn">The connection to create the transaction.</param>
        /// <param name="isolationLevel">The isolation level for the transaction.</param>
        /// <returns>The nested level of the transaction.</returns>
        public static int Enlist(DbConnection conn, IsolationLevel isolationLevel)
        {
            return Enlist(conn.BeginTransaction(isolationLevel));
        }

        /// <summary>
        /// Enlists a Transaction in the transaction manager.
        /// </summary>
        /// <param name="conn">The connection to create the transaction.</param>
        /// <returns>The nested level of the transaction.</returns>
        public static int Enlist(System.Data.Common.DbConnection conn)
        {
            return Enlist(conn.BeginTransaction());
        }

        /// <summary>
        /// Enlists a Transaction in the transaction pool.
        /// </summary>
        /// <param name="t">The trancsation to be enlisted.</param>
        /// <returns>The nested level of the transaction.</returns>
        public static int Enlist(DbTransaction t)
        {
            Monitor.Enter(_transactions);
            try
            {
                if (!_transactions.ContainsKey(t.Connection))
                {
                    _transactions.Add(t.Connection, new List<DbTransaction>());
                }
                _transactions[t.Connection].Add(t);
                return _transactions[t.Connection].Count;
            }
            finally
            {
                Monitor.Exit(_transactions);
            }
        }


        /// <summary>
        /// Removes the Transaction associated to the connection object from the transaction manager.
        /// </summary>
        /// <param name="conn">The connection instance associated with the transaction.</param>
        /// <param name="transaction">The transaction attached to the connection.</param>
        private static void DeEnlist(DbConnection conn,DbTransaction transaction)
        {
            lock (_transactions)
            {
                if (_transactions.ContainsKey(conn))
                {
                    List<DbTransaction> transactions = _transactions[conn];
                    if (transaction != null)
                    {
                        transactions.Remove(transaction);
                    }
                    else
                    {
                        transactions.Reverse();
                        foreach (DbTransaction tr in transactions)
                        {
                            tr.Rollback();
                        }
                        transactions.Clear();
                    }
                    if (transactions.Count == 0)
                        _transactions.Remove(conn);
                }
            }
        }
        /// <summary>
        /// Removes the Transaction from the transaction manager.
        /// </summary>
        /// <param name="connection">The connection to be removed.</param>
        public static void DeEnlist(DbConnection connection)
        {
            DeEnlist(connection, null);
        }
    }
}

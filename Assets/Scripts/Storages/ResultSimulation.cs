using Assets.Scripts.DataSerialiizers;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Assets.Scripts.Storages
{
    [Serializable]
    public class ResultSimulation : IStorage
    {
        [NonSerialized]
        private readonly IDataManager _dataManager;
        [NonSerialized]
        private string _path;

        private double[] _time;
        private double[] _nu;
        private double[] _E;

        private Vector<double>[] _satelliteMovement;
        private double[] _H;

        public ResultSimulation(IDataManager dataManager, double[] time, double[] nu, double[] E, Vector<double>[] satelliteMovement, double[] H)
        {
            _path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\", "Data", "ResultSimulation"));
            _dataManager = dataManager;
            _time = time;
            _nu = nu;
            _E = E;
            _satelliteMovement = satelliteMovement;
            _H = H;
        }

        // Использовать _dataManager для сохранения данных класса
        public void Save() => _dataManager.Write(this, _path);

        /// <summary>
        /// Формирует строковое представление сериализуемых полей класса в табличном виде.
        /// </summary>
        /// <remarks>
        /// Для массивов <c>double[]</c> элементы выводятся в одну строку, разделённые символом табуляции ('\t').
        /// Для массива векторов <c>Vector&lt;double&gt;[]</c> вывод производится по компонентам: сначала все первые компоненты векторов, затем все вторые и т.д.; каждая серия компонент занимает отдельную строку.
        /// Все числовые значения приводятся к типу <c>float</c> и форматируются с использованием <see cref="System.Globalization.CultureInfo.InvariantCulture"/>, чтобы разделителем дробной части была точка.
        /// Переход к следующему поля данных обозначается переводом строки.
        /// </remarks>
        /// <returns>Строка с данными, готовая для записи в текстовый файл.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(ToFloatRow(_time));
            sb.AppendLine(ToFloatRow(_nu));
            sb.AppendLine(ToFloatRow(_E));
            sb.AppendLine(ToFloatRow(_H));

            for (int comp = 0; comp < _satelliteMovement[0].Count; comp++)
            {
                for (int i = 0; i < _satelliteMovement.Length; i++)
                    sb.Append(((float)_satelliteMovement[i][comp]).ToString(CultureInfo.InvariantCulture)).Append('\t');
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string ToFloatRow(double[] array)
        {
            var sb = new StringBuilder();
            foreach (var x in array)
                sb.Append(((float)x).ToString(CultureInfo.InvariantCulture)).Append('\t');
            return sb.ToString().TrimEnd('\t');
        }
    }
}

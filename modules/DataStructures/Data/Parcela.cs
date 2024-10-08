namespace DataStructures.Data
{
    public class Parcela
    {
        #region Properties
        public List<Nehnutelnost> Nehnutelnosti { get; set; } = new List<Nehnutelnost>();       // iba referencie
        #endregion //Properties

        #region Constructor
        public Parcela()
        {
        }
        #endregion //Constructor
    }
}

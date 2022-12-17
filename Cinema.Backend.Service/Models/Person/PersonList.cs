namespace Template.Service.Models.Person
{
    /// <summary>
    /// A list of persons.
    /// </summary>
    public class PersonList : List<Person>
    {
        /// <summary>
        /// Initialize a new instance of the PersonList class.
        /// </summary>
        public PersonList()
            : base()
        {

        }

        /// <summary>
        /// Initialize a new instance of the PersonList class.
        /// </summary>
        public PersonList(IEnumerable<Person> list)
            : base(list)
        {

        }

        /// <summary>
        /// Gets a list of the IDs of the persons contained within the list.
        /// </summary>
        /// <returns>
        /// A list of the IDs contained within the list.
        /// </returns>
        public List<Guid> ToIds()
        {
            List<Guid> ids = new List<Guid>();

            foreach (Person cluster in this)
            {
                ids.Add(cluster.Id);
            }

            return ids;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">
        /// The pair to compare with the current object.
        /// </param>
        /// <returns>
        /// true if the specified pair is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if ((obj == null) || (this.GetType() != obj.GetType()))
            {
                return false;
            }

            PersonList list = (PersonList)obj;

            bool equals = false;

            if (this.Count == list.Count)
            {
                equals = true;

                for (int x = 0; x < this.Count; ++x)
                {
                    if (!this.Contains(list[x]))
                    {
                        equals = false;
                        break;
                    }
                }
            }

            return equals;
        }

        /// <summary>
        /// Gets the hash code for the current object.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 0;

            foreach (var item in this)
            {
                hash ^= item.GetHashCode();
            }

            return hash;
        }
    }
}

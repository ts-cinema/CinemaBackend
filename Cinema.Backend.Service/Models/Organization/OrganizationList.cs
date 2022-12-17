namespace Template.Service.Models.Organization
{
    /// <summary>
    /// A list of organizations.
    /// </summary>
    public class OrganizationList : List<Organization>
    {
        /// <summary>
        /// Initialize a new instance of the OrganizationList class.
        /// </summary>
        public OrganizationList()
            : base()
        {

        }

        /// <summary>
        /// Initialize a new instance of the OrganizationList class.
        /// </summary>
        public OrganizationList(IEnumerable<Organization> list)
            : base(list)
        {

        }

        /// <summary>
        /// Gets a list of the IDs of the organizations contained within the list.
        /// </summary>
        /// <returns>
        /// A list of the IDs contained within the list.
        /// </returns>
        public List<Guid> ToIds()
        {
            List<Guid> ids = new List<Guid>();

            foreach (Organization cluster in this)
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

            OrganizationList list = (OrganizationList)obj;

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

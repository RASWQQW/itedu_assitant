using System.ComponentModel.DataAnnotations.Schema;

namespace itedu_assitant.Model.Base
{
    public class Active
    {
        public int id { get; set; }
        public UserNumbers IsActive { get; set; }
        public int changeAmount { get; set; } = 0;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime LastInsert { get; set; }
    }
}

using itedu_assitant.DB;
using itedu_assitant.forsave.Methods;

namespace itedu_assitant.Controllers.Methods
{
    public class Builders
    {

        // It can be alsi user for other stuffs
        public Builders()
        {
        }
        static Wh_Instance ActiveContainer { get; set; } = null;

        public static Wh_Instance ImpleWhIns(dbcontext context, IEnumerable<EndpointDataSource> endpoints = null)
        {
            if (ActiveContainer == null)
                ActiveContainer = Wh_Instance.Create(context, endpoints);
            return ActiveContainer;
        }
    }
}

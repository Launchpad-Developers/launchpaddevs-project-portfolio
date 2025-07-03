using System;
using System.Threading.Tasks;
using AS.Forms.Controls.Validation;
using ASWorkoutTracker.Datastore;

namespace ASWorkoutTracker.Validation
{
    public class UniqueRecordNameRule<T, U> : IValidationRule<T> where U : new()
    {
        public string ValidationMessage { get; set; }
        public IASWTDatastoreService ASWTDatastoreService { get; set; }

        public bool Check(T value)
        {
            return true;
        }

        public async Task<bool> CheckAsync(T value)
        {
            if (value == null)
                return true;

            var str = value as string;

            var result = await ASWTDatastoreService.CanSaveUniqueRecord<U>(str);
            return result;
        }
    }
}

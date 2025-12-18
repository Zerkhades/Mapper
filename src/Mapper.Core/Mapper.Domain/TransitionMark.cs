using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Domain
{
    public sealed class TransitionMark : GeoMark
    {
        public Guid TargetGeoMapId { get; private set; }

        private TransitionMark() { }

        public TransitionMark(Guid geoMapId, double x, double y, string title, Guid targetGeoMapId, string? description = null)
            : base(geoMapId, GeoMarkType.Transition, x, y, title, description)
        {
            TargetGeoMapId = targetGeoMapId;
        }

        public void SetTarget(Guid targetGeoMapId) => TargetGeoMapId = targetGeoMapId;
    }
}

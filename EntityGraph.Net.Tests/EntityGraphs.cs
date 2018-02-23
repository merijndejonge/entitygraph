using OpenSoftware.EntityGraph.Net.Tests.Model;
using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net.Tests
{
    public static class EntityGraphs
    {
        public static EntityGraphShape SimpleGraphShape1 //MyGraph
        {
            get
            {
                return new EntityGraphShape()
                    .Edge<E, F>(e => e.F)
                    .Edge<F, E>(f => f.ESet);
            }
        }

        public static EntityGraphShape SimpleGraphShape2
        {
            get
            {
                return new EntityGraphShape()
                    .Edge<F, E>(f => f.ESet)
                    .Edge<Gh, G>(gh => gh.G)
                    .Edge<G, Gh>(g => g.GhSet);
            }
        }

        public static EntityGraphShape SimpleGraphShape3
        {
            get
            {
                return new EntityGraphShape()
                    .Edge<Gh, H>(gh => gh.H)
                    .Edge<H, Gh>(h => h.GhSet);
            }
        }

        public static EntityGraphShape SimpleGraphShapeFull
        {
            get
            {
                return new EntityGraphShape()
                    .Edge<E, F>(e => e.F)
                    .Edge<F, E>(f => f.ESet)
                    .Edge<Gh, G>(gh => gh.G)
                    .Edge<Gh, H>(gh => gh.H)
                    .Edge<G, Gh>(g => g.GhSet)
                    .Edge<H, Gh>(h => h.GhSet);
            }
        }

        public static EntityGraphShape CircularGraphShape1 // MyGraph
        {
            get
            {
                return new EntityGraphShape()
                    .Edge<A, B>(a => a.B)
                    .Edge<B, C>(b => b.C)
                    .Edge<C, D>(c => c.D)
                    .Edge<D, A>(d => d.A);
            }
        }

        public static EntityGraphShape CircularGraphFull
        {
            get
            {
                return new EntityGraphShape()
                    .Edge<A, B>(a => a.B)
                    .Edge<A, B>(a => a.BSet)
                    .Edge<B, A>(b => b.A)
                    .Edge<B, C>(b => b.C)
                    .Edge<C, D>(c => c.D)
                    .Edge<D, A>(d => d.A);
            }
        }
    }
}

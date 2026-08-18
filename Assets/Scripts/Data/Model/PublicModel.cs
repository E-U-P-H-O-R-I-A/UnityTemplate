using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    public abstract class PublicModel : ScriptableObject, IPublicModel
    {
        public abstract class Single<TScheme> : PublicModel where TScheme : PublicScheme, new()
        {
            [SerializeField] protected TScheme scheme = new();

            public TScheme GetScheme() =>
                scheme ??= new TScheme();
        }

        public abstract class Collection<TScheme> : PublicModel where TScheme : PublicScheme
        {
            [SerializeField] protected List<TScheme> schemes = new();

            public IReadOnlyList<TScheme> Schemes => schemes;

            public TScheme GetScheme<TType>(TType type) where TType : struct, Enum =>
                GetScheme(type.ToString());

            protected virtual TScheme GetScheme(string id) =>
                schemes.FirstOrDefault(scheme => scheme != null && scheme.ID == id);
        }
    }
}

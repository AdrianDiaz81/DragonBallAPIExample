import { useNavigate, useParams } from 'react-router-dom';
import { AFFILIATION_COLORS, FALLBACK_IMAGE } from '../utils/affiliationColors';
import { useCharacter } from '../hooks/useCharacter';

export function CharacterPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { character, loading, error } = useCharacter(id);

  if (loading) {
    return (
      <div className="min-h-screen bg-[#0a0a0a] flex items-center justify-center">
        <div className="w-12 h-12 border-4 border-orange-500 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (error || !character) {
    return (
      <div className="min-h-screen bg-[#0a0a0a] flex flex-col items-center justify-center text-red-400 gap-4">
        <span className="text-5xl">⚠️</span>
        <p>{error ?? 'Personaje no encontrado'}</p>
        <button onClick={() => navigate('/')} className="text-orange-400 hover:underline">
          ← Volver
        </button>
      </div>
    );
  }

  const badgeColor = (character.affiliation && AFFILIATION_COLORS[character.affiliation]) ?? 'bg-gray-600';

  return (
    <div className="min-h-screen bg-[#0a0a0a]">
      <header className="border-b border-[#1f2937] bg-[#0a0a0a]/80 backdrop-blur sticky top-0 z-10">
        <div className="max-w-7xl mx-auto px-4 py-4">
          <button
            onClick={() => navigate('/')}
            className="text-gray-400 hover:text-orange-400 transition-colors flex items-center gap-2 text-sm"
          >
            ← Volver a personajes
          </button>
        </div>
      </header>

      <main className="max-w-4xl mx-auto px-4 py-12">
        <div className="bg-[#111827] border border-[#1f2937] rounded-3xl overflow-hidden">
          <div className="relative h-72 bg-gradient-to-b from-[#1f2937] to-[#111827] flex items-end justify-center">
            <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_center,_rgba(249,115,22,0.15)_0%,_transparent_70%)]" />
            <img
              src={character.imageUrl}
              alt={character.name}
              className="h-full object-contain object-bottom relative z-10"
              onError={(e) => {
                (e.target as HTMLImageElement).src = FALLBACK_IMAGE;
              }}
            />
          </div>

          <div className="p-8">
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
              <h1 className="text-4xl font-black text-white">{character.name} {character.lastName}</h1>
              <span className={`${badgeColor} text-white text-sm px-4 py-1.5 rounded-full font-medium self-start`}>
                {character.affiliation}
              </span>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
              <Stat label="Raza" value={character.race} />
              <Stat label="Afiliación" value={character.affiliation} />
              <Stat label="Nivel de poder" value={`⚡ ${character.powerLevel.toLocaleString()}`} highlight />
            </div>

            {character.description && (
              <div className="mt-6">
                <p className="text-xs text-gray-500 uppercase tracking-wider mb-2">Descripción</p>
                <p className="text-gray-300 leading-relaxed">{character.description}</p>
              </div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}

function Stat({ label, value, highlight = false }: { label: string; value: string | undefined; highlight?: boolean }) {
  return (
    <div className="bg-[#1f2937] rounded-2xl p-4">
      <p className="text-xs text-gray-500 uppercase tracking-wider mb-1">{label}</p>
      <p className={`font-bold text-lg ${highlight ? 'text-orange-400' : 'text-white'}`}>{value ?? '—'}</p>
    </div>
  );
}

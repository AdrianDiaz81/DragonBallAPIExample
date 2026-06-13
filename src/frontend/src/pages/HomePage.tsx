import { useMemo, useState } from 'react';
import { CharacterGrid } from '../components/CharacterGrid';
import { FilterBar } from '../components/FilterBar';
import { SearchBar } from '../components/SearchBar';
import { useCharacters } from '../hooks/useCharacters';

export function HomePage() {
  const { characters, loading, error } = useCharacters();
  const [search, setSearch] = useState('');
  const [selectedRace, setSelectedRace] = useState('');
  const [selectedAffiliation, setSelectedAffiliation] = useState('');

  const races = useMemo(
    () => [...new Set(characters.map((c) => c.race).filter((r): r is string => r !== undefined))].sort(),
    [characters],
  );
  const affiliations = useMemo(
    () => [...new Set(characters.map((c) => c.affiliation).filter((a): a is string => a !== undefined))].sort(),
    [characters],
  );

  const filtered = useMemo(
    () =>
      characters.filter((c) => {
        const matchesSearch = c.name.toLowerCase().includes(search.toLowerCase());
        const matchesRace = selectedRace === '' || c.race === selectedRace;
        const matchesAffiliation = selectedAffiliation === '' || c.affiliation === selectedAffiliation;
        return matchesSearch && matchesRace && matchesAffiliation;
      }),
    [characters, search, selectedRace, selectedAffiliation],
  );

  return (
    <div className="min-h-screen bg-[#0a0a0a]">
      {/* Header */}
      <header className="border-b border-[#1f2937] bg-[#0a0a0a]/80 backdrop-blur sticky top-0 z-10">
        <div className="max-w-7xl mx-auto px-4 py-4 flex items-center gap-4">
          <span className="text-2xl font-black text-orange-500 tracking-tight">
            🐉 Dragon Ball
          </span>
          <span className="text-gray-600 text-xl">/</span>
          <span className="text-gray-300 font-medium">Personajes</span>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 py-8">
        {/* Hero */}
        <div className="mb-8">
          <h1 className="text-4xl font-black text-white mb-2">
            Personajes de{' '}
            <span className="text-transparent bg-clip-text bg-gradient-to-r from-orange-400 to-yellow-400">
              Dragon Ball
            </span>
          </h1>
          <p className="text-gray-400">
            {loading ? '...' : `${filtered.length} personajes encontrados`}
          </p>
        </div>

        {/* Controles */}
        <div className="flex flex-col sm:flex-row gap-3 mb-8">
          <SearchBar value={search} onChange={setSearch} />
          <FilterBar
            races={races}
            affiliations={affiliations}
            selectedRace={selectedRace}
            selectedAffiliation={selectedAffiliation}
            onRaceChange={setSelectedRace}
            onAffiliationChange={setSelectedAffiliation}
          />
        </div>

        {/* Contenido */}
        {loading && (
          <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-4">
            {Array.from({ length: 10 }).map((_, i) => (
              <div
                key={i}
                className="bg-[#111827] border border-[#1f2937] rounded-2xl overflow-hidden animate-pulse"
              >
                <div className="h-56 bg-[#1f2937]" />
                <div className="p-4 space-y-2">
                  <div className="h-4 bg-[#1f2937] rounded w-3/4" />
                  <div className="h-3 bg-[#1f2937] rounded w-1/2" />
                </div>
              </div>
            ))}
          </div>
        )}

        {error && (
          <div className="flex flex-col items-center justify-center py-24 text-red-400">
            <span className="text-5xl mb-4">⚠️</span>
            <p className="text-lg font-semibold">Error al cargar los personajes</p>
            <p className="text-sm text-gray-500 mt-1">{error}</p>
            <p className="text-sm text-gray-500 mt-1">
              Asegúrate de que la API está corriendo en{' '}
              <code className="text-orange-400">dotnet run --project src/Api</code>
            </p>
          </div>
        )}

        {!loading && !error && <CharacterGrid characters={filtered} />}
      </main>
    </div>
  );
}

interface Props {
  races: string[];
  affiliations: string[];
  selectedRace: string;
  selectedAffiliation: string;
  onRaceChange: (value: string) => void;
  onAffiliationChange: (value: string) => void;
}

export function FilterBar({
  races,
  affiliations,
  selectedRace,
  selectedAffiliation,
  onRaceChange,
  onAffiliationChange,
}: Props) {
  const selectClass =
    'bg-[#1f2937] border border-[#374151] text-white rounded-xl px-3 py-2.5 outline-none ' +
    'focus:border-orange-500 focus:ring-1 focus:ring-orange-500 transition-colors cursor-pointer text-sm';

  return (
    <div className="flex gap-3 flex-wrap">
      <select value={selectedRace} onChange={(e) => onRaceChange(e.target.value)} className={selectClass}>
        <option value="">Todas las razas</option>
        {races.map((r) => (
          <option key={r} value={r}>{r}</option>
        ))}
      </select>

      <select value={selectedAffiliation} onChange={(e) => onAffiliationChange(e.target.value)} className={selectClass}>
        <option value="">Todas las afiliaciones</option>
        {affiliations.map((a) => (
          <option key={a} value={a}>{a}</option>
        ))}
      </select>
    </div>
  );
}

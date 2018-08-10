// Copyright (c) 2006-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)
//
// File: ScrMarkerMapping.cs
// Responsibility: TE Team

using SIL.LCModel.Core.Text;
using SIL.LCModel.DomainServices;

namespace SIL.LCModel.DomainImpl
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// ScrMarkerMapping holds info about an Import mapping.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	internal partial class ScrMarkerMapping
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initialize the members of a ScrMarkerMapping from an ImportMappingInfo object
		/// </summary>
		/// <param name="info"></param>
		/// ------------------------------------------------------------------------------------
		internal void InitFromImportMappingInfo(ImportMappingInfo info)
		{
			BeginMarker = info.BeginMarker;
			EndMarker = info.EndMarker;
			Excluded = info.IsExcluded;
			Target = (int)info.MappingTarget;
			Domain = (int)info.Domain;
			StyleRA = info.Style == null ? m_cache.LangProject.FindStyle(info.StyleName) : info.Style;
			WritingSystem = info.WsId;
			NoteTypeRA = info.NoteType;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Get the members of a ScrMarkerMapping into an ImportMappingInfo object
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public ImportMappingInfo ToImportMappingInfo()
		{
			MarkerDomain domain = (MarkerDomain)Domain;
			if ((domain & MarkerDomain.DeprecatedScripture) != 0)
			{
				// If this mapping is for DeprecatedScripture, then clear
				// the DeprecatedScripture bit.
				domain ^= MarkerDomain.DeprecatedScripture;
			}

			if (Target == (int)MappingTargetType.DefaultParaChars)
			{
				return new ImportMappingInfo(BeginMarker, EndMarker, Excluded,
					MappingTargetType.TEStyle, domain, StyleUtils.DefaultParaCharsStyleName,
					WritingSystem, NoteTypeRA);
			}

			return new ImportMappingInfo(BeginMarker, EndMarker, Excluded,
				(MappingTargetType)Target, domain, StyleRA, WritingSystem, NoteTypeRA);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Retrieve the style name from the attached style if one exists
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string StyleName
		{
			get { return (StyleRA == null ? null : StyleRA.Name); }
		}
	}
}

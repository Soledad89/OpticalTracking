/***********************************************************************
BumpSpecularAdd.fs - Fragment shader for bump mapping and specular
lighting with additive term.
Copyright (c) 2010 Oliver Kreylos

This file is part of the Simple Scene Graph Renderer (SceneGraph).

The Simple Scene Graph Renderer is free software; you can redistribute
it and/or modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

The Simple Scene Graph Renderer is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
General Public License for more details.

You should have received a copy of the GNU General Public License along
with the Simple Scene Graph Renderer; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
***********************************************************************/

uniform sampler2D normalMap;
uniform sampler2D specularMap;
uniform sampler2D additiveMap;

varying vec3 eyeVec;
varying vec3 lightVec;

void main()
	{
	/* Get the local normal vector from the normal map: */
	vec3 normal=normalize(texture2D(normalMap,gl_TexCoord[0].st).rgb-vec3(0.5));
	// vec3 normal=vec3(0.0,0.0,1.0);
	
	/* Get the specular color from the specular map: */
	vec4 sSpecular=texture2D(specularMap,gl_TexCoord[0].st);
	// vec4 sSpecular=vec4(1.0,1.0,1.0,0.0);
	
	/* Calculate the diffuse lighting term: */
	vec4 color=vec4(0.0,0.0,0.0,0.0);
	float lightDist=length(lightVec);
	vec3 nLightVec=normalize(lightVec);
	float nl=dot(normal,nLightVec);
	if(nl>0.0)
		{
		/* Calculate the half-vector: */
		vec3 halfVec=normalize(nLightVec+normalize(eyeVec));
		
		/* Calculate the specular lighting term: */
		float nhv=dot(normal,halfVec);
		if(nhv>0.0)
			color+=gl_LightSource[0].specular*sSpecular*pow(nhv,gl_FrontMaterial.shininess);
		}
	
	/* Attenuate the light source's color contribution: */
	float lightAtt=1.0/((gl_LightSource[0].quadraticAttenuation*lightDist+gl_LightSource[0].linearAttenuation)*lightDist+gl_LightSource[0].constantAttenuation);
	color*=lightAtt;
	
	/* Calculate the additive term: */
	color+=texture2D(additiveMap,gl_TexCoord[0].st);
	
	/* Set the fragment color: */
	color[3]=sSpecular[3];
	gl_FragColor=color;
	}

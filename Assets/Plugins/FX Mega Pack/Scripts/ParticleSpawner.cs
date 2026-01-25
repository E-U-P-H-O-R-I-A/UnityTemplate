/****************************************
    ParticleSpawner.cs v1.0
    Copyright 2013 Unluck Software
    www.chemicalbliss.com

    GUI Buttons to view Particle Systems from list
*****************************************/

using UnityEngine;
using System;
using System.Collections.Generic;

public class ParticleSpawner : MonoBehaviour
{
    // Visible properties
    public GameObject[] particles;     // Particle systems to add a button for each
    public int maxButtons = 10;         // Maximum buttons per page
    public bool showInfo;

    // Hidden properties
    private int page = 0;               // Current page
    private int pages;                  // Number of pages
    private string currentPSInfo;       // Current particle info
    private GameObject currentPS;

    void Start()
    {
        // Sort particle system list alphabetically
        Array.Sort(particles, (g1, g2) => string.Compare(g1.name, g2.name));

        // Calculate number of pages
        pages = Mathf.CeilToInt((particles.Length - 1f) / maxButtons);
    }

    void OnGUI()
    {
        // Time Scale Vertical Slider
        Time.timeScale = GUI.VerticalSlider(
            new Rect(185, 50, 20, 150),
            Time.timeScale,
            2.0f,
            0.0f
        );

        // Check if pagination is needed
        if (particles.Length > maxButtons)
        {
            // Prev button
            if (GUI.Button(new Rect(20, (maxButtons + 1) * 18, 75, 18), "Prev"))
            {
                if (page > 0) page--;
                else page = pages;
            }

            // Next button
            if (GUI.Button(new Rect(95, (maxButtons + 1) * 18, 75, 18), "Next"))
            {
                if (page < pages) page++;
                else page = 0;
            }

            // Page label
            GUI.Label(
                new Rect(60, (maxButtons + 2) * 18, 150, 22),
                "Page " + (page + 1) + " / " + (pages + 1)
            );
        }

        // Toggle button for info
        showInfo = GUI.Toggle(new Rect(185, 20, 75, 25), showInfo, "Info");

        // System info
        if (showInfo)
        {
            GUI.Label(new Rect(250, 20, 500, 500), currentPSInfo);
        }

        // Calculate how many buttons on current page
        int pageButtonCount = particles.Length - (page * maxButtons);
        if (pageButtonCount > maxButtons)
            pageButtonCount = maxButtons;

        // Create buttons
        for (int i = 0; i < pageButtonCount; i++)
        {
            int index = i + (page * maxButtons);
            if (GUI.Button(
                new Rect(20, i * 18 + 18, 150, 18),
                particles[index].transform.name
            ))
            {
                if (currentPS)
                    Destroy(currentPS);

                GameObject go = Instantiate(particles[index]);
                currentPS = go;

                ParticleSystem ps = go.GetComponent<ParticleSystem>();
                PlayPS(ps, index + 1);
                InfoPS(ps, index + 1);
            }
        }
    }

    // Play particle system (resets time scale)
    void PlayPS(ParticleSystem ps, int nr)
    {
        Time.timeScale = 1f;
        ps.Play();
    }

    void InfoPS(ParticleSystem ps, int nr)
    {
        Renderer rend = ps.GetComponent<Renderer>();

        currentPSInfo =
            "System: " + nr + "/" + particles.Length + "\n" +
            "Name: " + ps.gameObject.name + "\n\n" +
            "Main PS Sub Particles: " + ps.transform.childCount + "\n" +
            "Main PS Materials: " + rend.materials.Length + "\n" +
            "Main PS Shader: " + rend.material.shader.name;

        // Plasma warning
        if (rend.materials.Length >= 2)
            currentPSInfo += "\n\n *Plasma not mobile optimized*";

        // Usage info
        currentPSInfo += "\n\n Use mouse wheel to zoom, click and hold to rotate";

        currentPSInfo = currentPSInfo.Replace("(Clone)", "");
    }
}

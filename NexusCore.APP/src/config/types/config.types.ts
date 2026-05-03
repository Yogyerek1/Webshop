import type { HomePageProps } from "../../pages/HomePage";

export interface BaseSection {
    id: string;
    enabled: boolean;
};


export type NewsSectionType = {
    type: 'News';
    props: HomePageProps;
};

export type PageSection = NewsSectionType /* | ... */;

export type HomePageType = {
    props: HomePageProps;
    pageSections?: PageSection[];
};

export interface AppConfig {
    homePage: HomePageType;
    theme: {
        primaryColor: string;
        showNavbar: boolean;
    };
};
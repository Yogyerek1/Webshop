import type { HomePageProps } from "../../pages/HomePage";
import type { NavBarProps } from "../../components/navigation/NavBar";

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
    navBar: NavBarProps;
    homePage: HomePageType;
    theme: {
        primaryColor: string;
    };
};
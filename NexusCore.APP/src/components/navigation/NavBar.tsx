import { useLocation } from "react-router-dom";
import { config } from "../../config/config";

type NavButton = {
    text?: string;
    path?: string;
};

type NavButtonProps = {
    bgColor?: string;
    selectedClass?: string;
    border?: string;
    padding?: string;
    textColor?: string;
    fontFamily?: string;
    fontWeight?: string;
    fontSize?: string;
    buttons: NavButton[];
};

type LogoProps = {
    width?: number;
    height?: number;
    logoIcon?: string;
    logoAlt?: string;
    logoText?: LogoTextProps;
    class?: string;
};

type LogoTextProps = {
    text?: string;
    textColor?: string;
    fontFamily?: string;
    fontWeight?: string;
    fontSize?: string;
};

export type NavBarProps = {
    props: NavBarMainProps;
};

type NavBarMainProps = {
    logo?: LogoProps; 
    bgColor?: string;
    border?: string;
    shadow?: string;
    navButtons?: NavButtonProps;
    cartIcon?: LogoProps;
    userIcon?: LogoProps;
    padding?: string;
    position?: string;
};

export const NavBar = ({ props }: NavBarProps) => {
    const location = useLocation();

    return (
        <div className={`${props.position ?? "absolute top-0"} flex flex-row items-center  h-fit ${props.bgColor} ${props.border} ${props.shadow} ${props.padding}`}>
            <div className="right-auto ml-1"><img src={props.logo?.logoIcon} alt={props.logo?.logoAlt} width={props.logo?.width ?? 55} height={props.logo?.height ?? 55} className={props.logo?.class} /></div>
            {props.logo?.logoText &&
                <div className={
                    `ml-2 
                    ${props.logo?.logoText.fontFamily} 
                    ${props.logo?.logoText.fontSize} 
                    ${props.logo?.logoText.fontWeight} 
                    ${props.logo?.logoText.textColor}
                    `}>
                    <span className="align-middle">{props.logo.logoText.text}</span>
                </div>
            }
            <div className="mx-auto flex flex-row gap-1 items-center">
                {config.navBar.props.navButtons &&
                    config.navBar.props.navButtons.buttons.map((item, index) => {
                        const isSelected = location.pathname === item.path;

                        return (
                            <div key={index}>
                                <a href={item.path ?? "/"} className={`
                                    ${config.navBar.props.navButtons?.bgColor} 
                                    ${config.navBar.props.navButtons?.border}
                                    ${config.navBar.props.navButtons?.padding}
                                    ${config.navBar.props.navButtons?.textColor}
                                    ${config.navBar.props.navButtons?.fontFamily}
                                    ${config.navBar.props.navButtons?.fontWeight}
                                    ${config.navBar.props.navButtons?.fontSize}
                                    ${isSelected ? config.navBar.props.navButtons?.selectedClass : ""}
                                    `}>
                                        {item.text}
                                    </a>
                            </div>
                        );
                    })
                }
            </div>
            <div><img src={props.cartIcon?.logoIcon} alt={props.cartIcon?.logoAlt} width={props.cartIcon?.width ?? 55} height={props.cartIcon?.height ?? 55} className={props.cartIcon?.class} /></div>
            <div><img src={props.userIcon?.logoIcon} alt={props.userIcon?.logoAlt} width={props.userIcon?.width ?? 55} height={props.userIcon?.height ?? 55} className={props.userIcon?.class} /></div>
        </div>
    );
};